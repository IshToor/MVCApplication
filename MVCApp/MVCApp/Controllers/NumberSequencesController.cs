using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCApp.Data;
using MVCApp.Models;

namespace MVCApp.Controllers
{
    public class NumberSequencesController : Controller
    {
        private readonly MVCAppContext _context;

        public NumberSequencesController(MVCAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Message = TempData["SubmissionMessage"] ?? "";
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!_context.SortedDataModel.Any(x => x.Id == id))
            {
                return NotFound();
            }

            var sortedDataModel = await _context.SortedDataModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sortedDataModel == null)
            {
                return NotFound();
            }
            
            return View(sortedDataModel);
        }

        public async Task<IActionResult> ViewSequences()
        {
            return View(await _context.SortedDataModel.ToListAsync());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!_context.SortedDataModel.Any(x => x.Id == id))
            {
                return NotFound();
            }

            var sortedDataModel = await _context.SortedDataModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sortedDataModel == null)
            {
                return NotFound();
            }

            return View(sortedDataModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sortedDataModel = await _context.SortedDataModel.FindAsync(id);
            
            if (sortedDataModel != null)
            {
                _context.SortedDataModel.Remove(sortedDataModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewSequences));
        }
        
        public IActionResult SubmitNumber(int currentNumber)
        {
            if (HttpContext.Request.Cookies.TryGetValue("NumberCookie", out var cookieValue))
            {
                HttpContext.Response.Cookies.Append("NumberCookie", cookieValue + "," + currentNumber);
            }
            else
            {
                HttpContext.Response.Cookies.Append("NumberCookie", currentNumber.ToString());
            }
            

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> SubmitAll(string sortType)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("NumberCookie", out var cookieValue)) return RedirectToAction(nameof(Index));
        
            var numberSequence = cookieValue.Split(',',StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            var watch = new Stopwatch();
        
            watch.Start();

            numberSequence = sortType == "Ascending" 
                ? numberSequence.OrderBy(x => x).ToList()
                : numberSequence.OrderByDescending(x => x).ToList();
        
            watch.Stop();

            var timeElapsed = watch.Elapsed.TotalMilliseconds + "ms";

            var numberSequenceText = string.Join(',',numberSequence);

            var sortedDataModel = new SortedDataModel
            {
                SortedNumberSequence = numberSequenceText,
                SortType = sortType,
                TimeElapsed = timeElapsed
            };
                        
            if (ModelState.IsValid)
            {
                _context.Add(sortedDataModel);
                await _context.SaveChangesAsync();
                TempData["SubmissionMessage"] = "Submission Successful";
            }
        
            HttpContext.Response.Cookies.Delete("NumberCookie");
            
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> ExportAllSequences()
        {
            if (!_context.SortedDataModel.Any()) 
            {
                //Add error message
                return RedirectToAction(nameof(ViewSequences));
            }
   
            const string fileName = "NumberSequences.json";
            const string mimeType = "application/json";
            await using var createStream = System.IO.File.Create(fileName);
            
            var sortedDataModels = await _context.SortedDataModel.ToListAsync();
            var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(sortedDataModels, jsonSerializerOptions);
            
            var fileBytes = Encoding.UTF8.GetBytes(jsonString);
            
            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
    }
}
