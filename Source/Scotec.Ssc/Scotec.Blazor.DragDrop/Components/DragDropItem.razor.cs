using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Blazor.DragDrop.Components
{
    public partial class DragDropItem<TItem> : DragDropComponent
    {
        public bool IsDraggable { get; set; }
    }
}
