﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using socialmvc.Data;
using socialmvc.Interfaces;
using socialmvc.Models;

namespace socialmvc.Controllers
{
	public class ClubController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly IClubRepository _clubRepository;

        public ClubController(ApplicationDbContext context, IClubRepository clubRepository)
		{
			_context = context;
			_clubRepository = clubRepository;
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
	}
}

