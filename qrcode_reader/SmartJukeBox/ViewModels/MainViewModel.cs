using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SmartJukeBox.Resources;

namespace SmartJukeBox.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Items.Add(new ItemViewModel() { LineOne = "Contagiarte", LineTwo = "descriçao sobre o contagiarte", LineThree = "http://www.viva-agenda.com/images/venues/1-1297605306-contagiarte.jpg" });
            this.Items.Add(new ItemViewModel() { LineOne = "Plano B", LineTwo = "descriçao sobre o ", LineThree = "http://artes.ucp.pt/b%26w/2009/images/planob_neon.jpg" });
            this.Items.Add(new ItemViewModel() { LineOne = "Piolho", LineTwo = "", LineThree = "http://3.bp.blogspot.com/-aKo3_lDIrck/Tszh-jWNlaI/AAAAAAAAAYk/oHwuToXZ9PY/s1600/piolho.jpg" });
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}