using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VisualTargetDemo
{
	public class VisualTargetPresentationSource : PresentationSource
	{
		public VisualTargetPresentationSource(HostVisual hostVisual)
		{
			_visualTarget = new VisualTarget(hostVisual);
		}

		public override Visual RootVisual
		{
			get => _visualTarget.RootVisual;

		    set
			{
				var oldRoot = _visualTarget.RootVisual;


				// Set the root visual of the VisualTarget.  This visual will
				// now be used to visually compose the scene.
				_visualTarget.RootVisual = value;

				// Hook the SizeChanged event on framework elements for all
				// future changed to the layout size of our root, and manually
				// trigger a size change.
			    if (value is FrameworkElement rootFe)
				{
					rootFe.SizeChanged += new SizeChangedEventHandler(root_SizeChanged);
					rootFe.DataContext = _dataContext;

					// HACK!
				    var myBinding = new Binding(_propertyName) {Source = _dataContext};
				    if (_propertyName != null)
					{
					    rootFe.SetBinding(TextBlock.TextProperty, myBinding);
					}
				}

				// Tell the PresentationSource that the root visual has
				// changed.  This kicks off a bunch of stuff like the
				// Loaded event.
				RootChanged(oldRoot, value);

				// Kickoff layout...
			    if (value is UIElement rootElement)
				{
					rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
					rootElement.Arrange(new Rect(rootElement.DesiredSize));
				}
			}
		}

		public object DataContext
		{
			get => _dataContext;
		    set
			{
				_dataContext = value;
			    if (_visualTarget.RootVisual is FrameworkElement rootElement)
				{
					rootElement.DataContext = _dataContext;
				}
			}
		}

		// HACK!
		public string PropertyName
		{
			get => _propertyName;
		    set
			{
				_propertyName = value;

			    if (_visualTarget.RootVisual is TextBlock rootElement)
				{
					if (!rootElement.CheckAccess())
					{
						throw new InvalidOperationException("What?");
					}

				    Binding myBinding = new Binding(_propertyName) {Source = _dataContext};
				    rootElement.SetBinding(TextBlock.TextProperty, myBinding);
				}
			}
		}

		public event SizeChangedEventHandler SizeChanged;

		public override bool IsDisposed => false;

	    protected override CompositionTarget GetCompositionTargetCore()
		{
			return _visualTarget;
		}

		private void root_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var handler = SizeChanged;
		    handler?.Invoke(this, e);
		}

		private readonly VisualTarget _visualTarget;
		private object _dataContext;
		private string _propertyName;
	}
}
