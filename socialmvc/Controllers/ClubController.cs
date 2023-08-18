﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using socialmvc.Data;
using socialmvc.Interfaces;
using socialmvc.Models;
using socialmvc.Repository;
using socialmvc.ViewModel;

namespace socialmvc.Controllers
{
	public class ClubController : Controller
	{
        private readonly IClubRepository _clubRepository;
		private readonly IPhotoService _photoService;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
		{
			_clubRepository = clubRepository;
			_photoService = photoService;
		}

		public async Task<IActionResult> Index()
		{
			IEnumerable<Club> clubs =await _clubRepository.GetAll();
			return View(clubs);
		}

		public async Task<IActionResult> Detail(int id)
		{
			Club club = await _clubRepository.GetByIdAsync(id);
			return View(club);
		}

		public IActionResult Create()
		{
			return View();
		}

        [HttpPost] //create club
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid)
            {
				var result = await _photoService.AddPhotoAsync(clubVM.Image);
				var club = new Club
				{
					Title = clubVM.Title,
					Description = clubVM.Description,
					Image = result.Url.ToString(),
                    Address = new Address
					{
						Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State,
                        Country = clubVM.Address.Country,
                    }
                };
                //return View(club);
				_clubRepository.Add(club);

				return RedirectToAction("Index");
            }
			else
			{
				ModelState.AddModelError("", "Image upload failed");
			}

			return View(clubVM);
            
        }

		//edit/update
        public async Task<IActionResult> Edit(int id)
        {
			var club = await _clubRepository.GetByIdAsync(id);
			if (club == null) return View("Error");

			var clubVM = new EditClubViewModel
			{
				Title = club.Title,
				Description = club.Description,
				AddressId = club.AddressId,
				Address = club.Address,
				URL = club.Image,
				ClubCategory = club.ClubCategory
			};

			return View(clubVM);
        }


		[HttpPost]
		public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Failed to edit club");
				return View("Edit", clubVM);
			}

			var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

			if(userClub != null)
			{
                try
                {
                    //delete previous image
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete image");
                    return View(clubVM);
                }

                var photoResul = await _photoService.AddPhotoAsync(clubVM.Image);

                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = photoResul.Url.ToString(),
                    AddressId = clubVM.AddressId,
                    Address = clubVM.Address
                };

                _clubRepository.Update(club);

                return RedirectToAction("Index");
			}
			else
			{
				return View(clubVM); 
			}
				
		}

		//delete
		public async Task<IActionResult> Delete(int id)
		{
			var clubDetails = await _clubRepository.GetByIdAsync(id);
			if (clubDetails == null) return View("Error");
			return View(clubDetails);
		}

		[HttpPost, ActionName("Delete")]

		public async Task<IActionResult> DeleteClub(int id)
		{
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails == null) return View("Error");

			_clubRepository.Delete(clubDetails);
			return RedirectToAction("Index");
        }
    }
}

