using Blackjack.Domain;
using Blackjack.Mvc.DAL;
using Blackjack.Mvc.Hubs;
using Blackjack.Mvc.Models;
using Blackjack.Mvc.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Blackjack.Mvc.Controllers
{
    public class GameController : Controller
    {
        private LiveBlackjackGame Game;

        private LiveBlackjackContext _blackjackContext;
        private LiveBlackjackContext BlackjackContext
        {
            get
            {
                if (_blackjackContext == null)
                    _blackjackContext = new LiveBlackjackContext();

                return _blackjackContext;
            }
        }

        private BlackjackGamePlayer _player;
        private BlackjackGamePlayer Player
        {
            get
            {
                if (_player == null)
                    _player = Game?.Players?.FirstOrDefault(a => a.Id == Session.SessionID);

                return _player;
            }
        }

        public ActionResult Index(string id)
        {            
            Game = BlackjackContext.GetGame(id);

            if (Game == null)
                return RedirectToAction("index", "lobby");

            return View("index", new BlackjackGameViewModel(Game, Player?.Id));
        }

        public ActionResult Refresh(string gameId)
        {
            Game = BlackjackContext.GetGame(gameId);

            if (Game == null)
                return new EmptyResult();

            return PartialView("_Game", new BlackjackGameViewModel(Game, Player?.Id));
        }

        public ActionResult Deal(string gameId)
        {
            try
            {
                Game = BlackjackContext.GetGame(gameId);
                Game.StartRound();
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        public ActionResult EndRound(string gameId)
        {
            try
            {
                Game = BlackjackContext.GetGame(gameId);
                Game.EndRound();
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        public ActionResult RemoveGamePlayer(string gameId)
        {
            try
            {
                Game = BlackjackContext.GetGame(gameId);
                Game.RemovePlayer(Player);
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        public ActionResult ForceStand(string gameId)
        {
            try
            {
                Game = BlackjackContext.GetGame(gameId);
                Game.ForceCurrentActionToStand();
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }        

        public ActionResult PlayerBetRequest(string gameId, string betAmount)
        {
            try
            {
                double bet = 0;
                if (!double.TryParse(betAmount, out bet))
                    throw new Exception("Invalid bet");

                Game = BlackjackContext.GetGame(gameId);
                Game.PlayerWagerRequest(Player, bet);
                Session["WagerAmount"] = bet;
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        public ActionResult PlayerJoinRequest(string gameId, string playerName, string _seatNo)
        {
            try
            {
                int seatNo = 0;
                if (!int.TryParse(_seatNo, out seatNo))
                    throw new Exception("Invaid seat number");

                Game = BlackjackContext.GetGame(gameId);

                double balance = 1000;
                if (HttpContext.Session["Balance"] == null || (double)HttpContext.Session["Balance"] <= 0)
                    HttpContext.Session["Balance"] = balance;
                else
                    balance = (double)HttpContext.Session["Balance"];

                var account = new PlayerAccount(
                    id: HttpContext.Session.SessionID,
                    startingBalance: balance);

                Game.AddPlayer(account, playerName, seatNo);

                Save();
                return PartialView("_PlayerChatInput", seatNo);
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        public ActionResult PlayerActionRequest(string gameId, string request)
        {
            try
            {
                Game = BlackjackContext.GetGame(gameId);
                Game.PlayerActionRequest(Player, request);
                Save();
                return Content("Ok");
            }
            catch (Exception exception)
            {
                return Content(exception.Message);
            }
        }

        private void Save()
        {
            if (Game == null)
                throw new InvalidOperationException("Cannot load game");

            BlackjackContext.SaveGameRoom(Game);

            if (Player != null)
            {
                HttpContext.Session["Balance"] = Player.Account.Balance;
            }

            GlobalHost.ConnectionManager
                .GetHubContext<BlackjackGameRoomHub>()?
                .Clients?
                .Group(Game.Id.ToString())?
                .refresh();
        }
    }
}

