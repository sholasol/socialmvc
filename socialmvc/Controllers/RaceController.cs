using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using socialmvc.Data;
using socialmvc.Interfaces;
using socialmvc.Models;
using socialmvc.Repository;
using socialmvc.ViewModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace socialmvc.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll(); 
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State,
                        Country = raceVM.Address.Country,
                    }
                };
                _raceRepository.Add(race);

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Image upload failed");
            }

            return View(raceVM);
        }


        //edit/update
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null) return View("Error");

            var raceVM = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory
            };

            return View(raceVM);
        }




        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ModelState.AddModelError("", "Failed to edit club");
        //        return View("Edit", clubVM);
        //    }

        //    var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

        //    if (userClub != null)
        //    {
        //        try
        //        {
        //            //delete previous image
        //            await _photoService.DeletePhotoAsync(userClub.Image);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "Could not delete image");
        //            return View(clubVM);
        //        }

        //        var photoResul = await _photoService.AddPhotoAsync(clubVM.Image);

        //        var club = new Club
        //        {
        //            Id = id,
        //            Title = clubVM.Title,
        //            Description = clubVM.Description,
        //            Image = photoResul.Url.ToString(),
        //            AddressId = clubVM.AddressId,
        //            Address = clubVM.Address
        //        };

        //        _clubRepository.Update(club);

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View(clubVM);
        //    }

        //}





    }
}

 