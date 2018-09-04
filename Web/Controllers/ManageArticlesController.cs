﻿
using Articles.WriteSide.Commands;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Web.Models.Articles;

namespace Web.Controllers
{
    //[Authorize(Roles = "Administrators, Editors")]
    [Route("api/[controller]/[action]")]
    public class ManageArticlesController : Controller
    {
        private IMapper _mapper;
        private ArticlesServiceClient _articlesService;

        public ManageArticlesController(IMapper mapper)
        {
            _mapper = mapper;
            _articlesService = new ArticlesServiceClient();
        }

        #region Articles

        [HttpGet("{categoryId}")]
        public IActionResult ManageArticles(Guid categoryId)
        {
            ArticleDto[] dto = _articlesService.GetArticlesByCategoryIdAsync(categoryId, 1, 10).Result;
            var model = _mapper.Map<ArticleItemViewModel[]>(dto);
            return Ok(model); 
        }

        [HttpGet]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IDeleteArticleCommand>(new
            {
                id
            });
            return Ok();
        }

        [HttpPost]
        public IActionResult AddArticle(AddArticleViewModel model)
        {
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult EditArticle(Guid id)
        {
            ArticleDto dto = _articlesService.GetArticleByIdAsync(id).Result;
            var model = _mapper.Map<EditArticleViewModel>(dto);
            return Ok(model); 
        }
         
        [HttpPut("{id}")] 
        public async Task<IActionResult> UpdateArticle(Guid id, [FromBody]EditArticleViewModel model)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IUpdateArticleCommand>(new
            {
                model.Id,
                model.Body
            });
            return Ok();
        }
        #endregion

        #region Categories
        [HttpGet]
        public IActionResult ManageCategories()
        {
            CategoryDto[] dto = _articlesService.GetCategoriesAsync().Result;
            CategoryItemViewModel[] model = _mapper.Map<CategoryItemViewModel[]>(dto);
            return Ok(model); 
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IDeleteArticleCommand>(new
            {
                Id = id
            });
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryViewModel model)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IInsertCategoryCommand>(new
            {
                model.Description,
                model.ImageUrl,
                model.Importance,
                model.Title
            });
            return Ok();
        }

        [HttpGet]
        public IActionResult EditCategory(Guid id)
        {
            CategoryDto dto = _articlesService.GetCategoryByIdAsync(id).Result;
            var model = _mapper.Map<EditCategoryViewModel>(dto);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, EditCategoryViewModel model)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IUpdateCategoryCommand>(new
            {
                model.Id,
                model.Description,
                model.ImageUrl,
                model.Importance,
                model.Title

            });
            return Ok();
        }
        #endregion

        #region Comments

        [HttpGet("{articleId}")]
        public IActionResult ManageComments(Guid articleId)
        {
            CommentDto[] dto = _articlesService.GetCommentsByArticleIdAsync(articleId, 1, 20).Result;
            var model = _mapper.Map<ManageCommentItemViewModel[]>(dto);
            return Ok(model); 
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IDeleteCommentCommand>(new
            {
                Id = id
            });
            return Ok();
        }

        [HttpGet]
        public IActionResult EditComment(Guid id)
        {
            CommentDto dto = _articlesService.GetCommentByIdAsync(id).Result;
            var model = _mapper.Map<EditCommentViewModel>(dto);
            return Ok(model); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, EditCommentViewModel model)
        {
            var endPoint = await BusConfigurator.GetEndPointAsync(RabbitMqConstants.ArticleWriteServiceQueue);
            await endPoint.Send<IUpdateCommentCommand>(new
            {
                model.Id,
                model.AddedBy,
                model.AddedDate,
                model.Comment,
                model.UserIp
            });
            return Ok();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
