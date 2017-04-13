using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElevenNote.Web.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        public ActionResult Index()
        {
            var service = CreateNoteService();

            var model = service.GetNotes();
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteCreate model)
        {
            if (!ModelState.IsValid) return View(model);

            var service = CreateNoteService();


            if (service.CreateNote(model))
            {
                TempData["SaveResult"] = "Your note was successfully created!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your note could not be created.");

            return View(model);

        }

        public ActionResult Details(int id)
        {
            var service = CreateNoteService();
            var model = service.GetNoteById(id);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var service = CreateNoteService();
            var detail = service.GetNoteById(id);
            var model =
                new NoteEdit
                {
                    NoteId = detail.NoteId,
                    Title = detail.Title,
                    Content = detail.Content
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NoteEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.NoteId != id)
            {
                ModelState.AddModelError("", "ID Mismatch");
                return View(model);
            }

            var service = CreateNoteService();


            if (service.UpdateNote(model))
            {
                TempData["SaveResult"] = "Your note was successfully updated!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your note could not be updated.");

            return View(model);
        }

        private NoteService CreateNoteService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new NoteService(userId);
            return service;
        }
    }
}