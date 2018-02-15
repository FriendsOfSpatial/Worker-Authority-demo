using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Improbable;
using Improbable.Collections;
using Improbable.Demo;
using Improbable.Worker;

namespace Managed
{
    class HeadColorProcessor
    {
        private uint thisWorkerColorId;
        private Connection connection;

        public HeadColorProcessor(uint colorId, Connection connection)
        {
            thisWorkerColorId = colorId;
            this.connection = connection;
        }

        public void OnAuthChangeOnComponentHeadColor(AuthorityChangeOp op)
        {
            if (op.Authority == Authority.Authoritative)
            {
                var update = new Improbable.Demo.HeadColor.Update();
                update.colorId = thisWorkerColorId;
                connection.SendComponentUpdate(op.EntityId, update);
            }
        }

        public void OnCheckOutUpdate(ComponentUpdateOp<Improbable.Demo.CheckOutColor> op)
        {
            if (op.Update.Get().ping.Count > 0)
            {
                connection.SendCommandRequest(op.EntityId, new CheckOutColor.Commands.SendColorId.Request(thisWorkerColorId), 0);
            }
        }
    }
}
