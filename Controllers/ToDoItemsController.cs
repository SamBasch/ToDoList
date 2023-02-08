using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.Services.Interfaces;

namespace ToDoList.Controllers
{

    [Authorize]
    public class ToDoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITdListService _listService;
       

        public ToDoItemsController(ApplicationDbContext context, UserManager<AppUser> userManager, ITdListService listService)
        {
            _context = context;
            _userManager = userManager;
            _listService = listService; 
        }

        // GET: ToDoItems
        public async Task<IActionResult> Index(int? accessoryId)
        {
            string? userId = _userManager.GetUserId(User)!;

            IEnumerable<ToDoItem> tdItems = await _context.ToDoItems.Where(t => t.AppUserId == userId).Include(t => t.Accessories).ToListAsync();

            List<Accessory> accessories = await _context.Accessories.Where(a => a.AppUserId == userId).ToListAsync();

            if (accessoryId == null)
            {

                tdItems = await _context.ToDoItems.Where(t => t.AppUserId == userId).Include(t => t.Accessories).ToListAsync();


            }
            else
            {
                tdItems = (await _context.Accessories.Include(a => a.ToDoItems).FirstOrDefaultAsync(a => a.AppUserId == userId && a.Id == accessoryId))!.ToDoItems.ToList();
            }


            ViewData["PersonList"] = new SelectList(accessories, "Id", "Name", accessoryId);


            //var applicationDbContext = _context.ToDoItems.Include(t => t.AppUser);

            return View(tdItems);
        }

        // GET: ToDoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDoItems == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // GET: ToDoItems/Create
        public async Task<IActionResult>  Create()
        {
            //ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");

            string? userId = _userManager.GetUserId(User);

            IEnumerable<Accessory> personList = await _context.Accessories.Where(a => a.AppUserId == userId).ToListAsync();

            ViewData["PersonList"] = new MultiSelectList(personList, "Id", "Name");


            return View();
        }









        // POST: ToDoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUserId,Name,DateCreated,DueDate,Completed")] ToDoItem toDoItem, IEnumerable<int> selected)
        {

            ModelState.Remove("AppUserId");


            if (ModelState.IsValid)
            {

                if(toDoItem.DueDate != null)
                {
                    toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
                }



                toDoItem.AppUserId = _userManager.GetUserId(User);


                toDoItem.DateCreated = DateTime.UtcNow;
        

                _context.Add(toDoItem);
               await _context.SaveChangesAsync();

                await _listService.AddtdItemToAccessoriesAsync(selected, toDoItem.Id);

               return RedirectToAction(nameof(Index));
            }
            //ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", toDoItem.AppUserId);
            return View(toDoItem);
        }

        // GET: ToDoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDoItems == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems.Include(t => t.Accessories).FirstOrDefaultAsync(t => t.Id == id);

            string? userId = _userManager.GetUserId(User);  

            IEnumerable<Accessory> personList = await _context.Accessories.Where(a => a.AppUserId == userId).ToListAsync();

            IEnumerable<int> currentPersons = toDoItem!.Accessories.Select(a => a.Id);

            ViewData["PersonList"] = new MultiSelectList(personList, "Id", "Name", currentPersons);

            if (toDoItem == null)
            {
                return NotFound();
            }
        
            return View(toDoItem);
        }

        // POST: ToDoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name,DateCreated,DueDate,Completed")] ToDoItem toDoItem, IEnumerable<int> selected)
        {
            if (id != toDoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {


                    if(toDoItem.DueDate != null)
                    {

                        toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
                    }

                    if (toDoItem.DueDate != null)
                    {

                        toDoItem.DateCreated = DateTime.SpecifyKind(toDoItem.DateCreated, DateTimeKind.Utc);
                    }

                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();

                    if (selected != null)
                    {
                        await _listService.RemoveAlltdItemAccessoriesAsync(toDoItem.Id);

                        await _listService.AddtdItemToAccessoriesAsync(selected, toDoItem.Id);

                        _context.Update(toDoItem);
                        await _context.SaveChangesAsync();
                    }

                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
         
            return View(toDoItem);
        }

        // GET: ToDoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ToDoItems == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // POST: ToDoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ToDoItems'  is null.");
            }
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id)
        {
          return (_context.ToDoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
