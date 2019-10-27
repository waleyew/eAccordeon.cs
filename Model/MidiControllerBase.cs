using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAccordeon.Model
{
    public abstract class MidiControllerBase
    {
        EAccordeon eAccordeon;
        public MidiControllerBase(EAccordeon eAccordeon)
        {
            this.eAccordeon = eAccordeon;
        }
        public EAccordeon EAccordeon
        {
            get { return eAccordeon; }
        }

        public string DebugText { get; internal set; }

        public abstract string Caption { get; }

        public abstract bool IsStarted { get; }

        public abstract void Start();

        public abstract void Stop();
    }
}
