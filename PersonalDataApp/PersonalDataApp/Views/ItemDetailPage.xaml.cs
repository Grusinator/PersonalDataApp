﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PersonalDataApp.Models;
using PersonalDataApp.ViewModels;

namespace PersonalDataApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemDetailPage : ContentPage
	{
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var datapoint = new Datapoint{};

            viewModel = new ItemDetailViewModel(datapoint);
            BindingContext = viewModel;
        }
    }
}