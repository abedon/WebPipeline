/* 
 * WebPipeline 
 * https://github.com/abedon/WebPipeline 
 * 
 * Copyright (c) 2017 Dong Chen
 * Licensed under the Apache 2.0 License. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface ICompositionPart
	{
		IGlobalOrderRepository Process(IGlobalOrderRepository globalOrderRepository, string controllerName);

        IGlobalOrderRepository Next(IGlobalOrderRepository globalOrderRepository, string currentPartName, string controllerName);
    }
}
