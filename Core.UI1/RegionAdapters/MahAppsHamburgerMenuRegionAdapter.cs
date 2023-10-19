using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UI.RegionAdapters
{
    public class MahAppsHamburgerMenuRegionAdapter : RegionAdapterBase<HamburgerMenu>
    {
        public ObservableCollection<HamburgerMenuItemBase> Records { get; set; } = new ObservableCollection<HamburgerMenuItemBase>();

        public MahAppsHamburgerMenuRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }

        protected override void Adapt(IRegion region, HamburgerMenu regionTarget)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }
            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            region.Views.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var view in args.NewItems)
                    {
                        AddViewToRegion(view, regionTarget);
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var view in args.OldItems)
                    {
                        RemoveViewFromRegion(view, regionTarget);
                    }
                }
            };
        }

        private void AddViewToRegion(object view, HamburgerMenu regionTarget)
        {
            if (view is HamburgerMenuItemCollection hmItem)
            {
                if (regionTarget.ItemsSource == null)
                {
                    regionTarget.ItemsSource = Records;
                }

                foreach (var item in hmItem)
                {
                    Records.Add(item);
                }

                if (regionTarget.SelectedItem == null)
                {
                    regionTarget.SelectedItem = view;
                }
            }
        }

        private void RemoveViewFromRegion(object view, HamburgerMenu regionTarget)
        {
            if (view is HamburgerMenuItemCollection hmItem)
            {
                foreach (var item in hmItem)
                {
                    Records.Remove(item);
                }
            }
        }

    }
}
