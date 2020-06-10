using System;
using System.Reflection;
using Windows.UI.Xaml;

namespace Lensman
{
    public interface IViewLocator
    {
        FrameworkElement ViewFor(object viewModel);
    }

    public class ViewLocator : IViewLocator
    {
        public FrameworkElement ViewFor(object viewModel)
        {
            var view = $"{viewModel.GetType().Namespace}.View";

            var viewType = Assembly.GetExecutingAssembly().GetType(view, true);

            return Activator.CreateInstance(viewType) as FrameworkElement;
        }
    }
}
