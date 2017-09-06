using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Notes_Instructions
{
    public partial class NoteListModel:BaseNopModel
    {
        public GridModel<NoteModel> Notes { get; set; }
        public int WorkLanguageId { get; set; }
    }
}