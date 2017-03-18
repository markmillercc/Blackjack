using Blackjack.Mvc.DAL;
using Blackjack.Mvc.Models;
using Blackjack.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blackjack.Mvc.Controllers
{
    public class LobbyController : Controller
    {
        private BlackjackContext _blackjackContext;
        private BlackjackContext BlackjackContext
        {
            get
            {
                if (_blackjackContext == null)
                    _blackjackContext = new BlackjackContext();

                return _blackjackContext;
            }
        }
        
        public ActionResult Index()
        {
            var gamelist = BlackjackContext
                .GetGameList()
                .Select(a => new BlackjackGameAsListItemViewModel(a))
                .ToList();
            return View(gamelist);
        }

        [HttpPost]
        public ActionResult CreateNewGame(FormCollection collection)
        {
            try
            {
                var name = collection["gameName"];
                var minBet = int.Parse(collection["minBet"]);
                var maxBet = int.Parse(collection["maxBet"]);

                var game = new Models.LiveBlackjackGame(minBet, maxBet, 30, 10);
                game.Title = name;
                BlackjackContext.SaveGameRoom(game);                
                return RedirectToAction("index", "game", new { id = game.Id });
            }
            catch
            {
                return View();
            }
        }





        // GET: Lobby/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Lobby/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lobby/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {                
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Lobby/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Lobby/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Lobby/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Lobby/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
