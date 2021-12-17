using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodes.Models;

namespace Nodes.ViewModels
{
    public class EdgeViewModel : ViewModelBase
    {
        private Edge _edge;

        public EdgeViewModel(Edge e)
        {
            _edge = e;
        }

        public Edge Edge => _edge;
    }
}
