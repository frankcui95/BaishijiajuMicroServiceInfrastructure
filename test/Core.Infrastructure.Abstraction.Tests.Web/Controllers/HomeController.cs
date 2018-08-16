﻿using Core.Infrastructure.Abstraction.Tests.Web.EFCore;
using Core.Messages.Bus;
using Core.PersistentStore.Repositories;
using Core.PersistentStore.Repositories.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Infrastructure.Abstraction.Tests.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(
            [FromServices]IRepository<WebEntity> repository,
            [FromServices]IRepository<WebEntity, int> repositoryWithKey,
            [FromServices]IAsyncRepository<WebEntity> asyncRepository,
            [FromServices]IAsyncRepository<WebEntity, int> asyncRepositoryWithKey)
        {
            var repositoryStatus = "OK";
            try
            {
                var hashSet = new HashSet<DbContext>
            {
                repository.GetDbContext(),
                repositoryWithKey.GetDbContext(),
                asyncRepository.GetDbContext(),
                asyncRepositoryWithKey.GetDbContext()
            };
                Debug.Assert(hashSet.Single() != null);
            }
            catch (Exception)
            {
                repositoryStatus = "Error";
            }
            return Json(new
            {
                RepositoryStatus = repositoryStatus
            });
        }
    }
}
