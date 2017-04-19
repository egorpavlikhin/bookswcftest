using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using KishTestProject.DataAccess;

namespace KishTestProject
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IBookService, BookService>(),
                Component.For<KishTestContext, KishTestContext>().LifestyleTransient(),
                Component.For<IBooksRepository, BooksRepository>(),
                Component.For<IBookRentsRepository, BookRentsRepository>()
                );
        }
    }
}