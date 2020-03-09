using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Todo.Data;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoLists;
using Todo.Models.TodoLists;
using Todo.Services;

namespace Todo.Controllers
{
    [Authorize]
    public class TodoListController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserStore<IdentityUser> userStore;
        private readonly IGravatarClient gravatarClient;

        public TodoListController(ApplicationDbContext dbContext, IUserStore<IdentityUser> userStore, IGravatarClient gravatarClient)
        {
            this.dbContext = dbContext;
            this.userStore = userStore;
            this.gravatarClient = gravatarClient;
        }

        public IActionResult Index()
        {
            var userId = User.Id();
            var todoLists = dbContext.RelevantTodoLists(userId);
            var viewmodel = TodoListIndexViewmodelFactory.Create(todoLists);
            return View(viewmodel);
        }

        public async Task<IActionResult> Detail(int todoListId)
        {
            var todoList = dbContext.SingleTodoList(todoListId);
            var viewmodel = TodoListDetailViewmodelFactory.Create(todoList);

            try
            {
                foreach (var group in viewmodel.Items.GroupBy(x => new { x.ResponsibleParty.Email, x.ResponsibleParty.UserName }))
                {
                    var profile = await gravatarClient.GetGravatarProfile(group.Key.Email, group.Key.UserName, new CancellationToken());
                    foreach (var item in group)
                    {
                        item.ResponsibleParty.UserName = profile.DisplayName;
                        item.ResponsibleParty.ThumbnailUrl = profile.ThumbnailUrl;
                        item.ResponsibleParty.HasGravatarProfile = profile.DoesExist;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("There is a connection problem with Gravatar API, please check your configuration");
            }

            return View(viewmodel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TodoListFields());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoListFields fields)
        {
            if (!ModelState.IsValid) { return View(fields); }

            var currentUser = await userStore.FindByIdAsync(User.Id(), CancellationToken.None);

            var todoList = new TodoList(currentUser, fields.Title);

            await dbContext.AddAsync(todoList);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Create", "TodoItem", new {todoList.TodoListId});
        }
    }
}