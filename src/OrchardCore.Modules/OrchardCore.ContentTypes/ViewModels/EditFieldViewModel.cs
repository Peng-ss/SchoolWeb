﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement.Metadata.Models;

namespace OrchardCore.ContentTypes.ViewModels
{
    public class EditFieldViewModel
    {
        /// <summary>
        /// The technical name of the field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display name of the field
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        [BindNever]
        public ContentPartFieldDefinition PartFieldDefinition { get; set; }

        [BindNever]
        public dynamic Editor { get; set; }
    }
}