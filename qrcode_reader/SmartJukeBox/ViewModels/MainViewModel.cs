﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SmartJukeBox.Resources;
using System.Windows;

namespace SmartJukeBox.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.Bands = new ObservableCollection<BandViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<BandViewModel> Bands { get; private set; }

        private string spotID;

        public string SpotID
        {
            get
            {
                return spotID;
            }
            set
            {
                if (spotID != value)
                {
                    spotID = value;
                    NotifyPropertyChanged("SpotID");
                    NotifyPropertyChanged("SpotPivotItemVisible");
                    NotifyPropertyChanged("CheckPivotItemVisible");
                }
            }
        }

        public Visibility SpotPivotItemVisible
        {
            get
            {
                return string.IsNullOrEmpty(spotID) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility CheckPivotItemVisible
        {
            get
            {
                return string.IsNullOrEmpty(spotID) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string _sampleProperty = "Sample Runtime Property Value";
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

            this.Bands.Add(new BandViewModel() { Name = "add band" });
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