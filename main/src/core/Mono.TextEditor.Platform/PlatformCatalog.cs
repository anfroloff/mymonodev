// 
// PlatformCatalog.cs
//  
// Author:
//       David Pugh <dpugh@microsoft.com>
// 
// Copyright (c) 2007 Novell, Inc (http://www.novell.com)
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Platform
{
    public class PlatformCatalog
    {
        public static PlatformCatalog Instance = new PlatformCatalog();

        public CompositionContainer CompositionContainer { get; }

        public ITextBufferFactoryService2 TextBufferFactoryService { get; }

        private PlatformCatalog()
        {
            var container = PlatformCatalog.CreateContainer();
            container.SatisfyImportsOnce(this);

            this.CompositionContainer = container;
            this.TextBufferFactoryService = (ITextBufferFactoryService2)_textBufferFactoryService;
        }

        private static CompositionContainer CreateContainer()
        {
            // TODO: Read these from manifest.addin.xml?
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(PlatformCatalog).Assembly));

            // Add other assemblies from which we expect to get MEF objects
            // TODO: add some mechanism to allow these to be updated at runtime.
            string[] assemblyNames =
                {
                };

            IEnumerable<Assembly> assembliesToCompose = assemblyNames.Select(name => Assembly.Load(name));
            foreach (Assembly curAssembly in assembliesToCompose)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(curAssembly));
            }

            //Create the CompositionContainer with the parts in the catalog
            CompositionContainer container = new CompositionContainer(catalog);

            return container;
        }

        [Import]
        internal ITextBufferFactoryService _textBufferFactoryService { get; set; }
    }
}