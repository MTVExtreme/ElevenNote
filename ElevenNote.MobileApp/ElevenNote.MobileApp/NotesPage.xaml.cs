using ElevenNote.MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElevenNote.MobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : ContentPage
    {
        private List<NoteListItemViewModel> Notes { get; set; }

        public NotesPage()
        {
            InitializeComponent();
            SetupUi();
        }

        /// <summary>
        /// Sets up user Interface without making designer confused
        /// </summary>

        private void SetupUi()
        {
            lvwNotes.IsPullToRefreshEnabled = true;
            lvwNotes.Refreshing += async (o, args) =>
            {
                await PopulateNotesList();
                lvwNotes.IsRefreshing = false;
                lblNoNotes.IsVisible = !Notes.Any();
            };

            //Add New Note icon to title bar.
            this.ToolbarItems.Add(new ToolbarItem("Add", null, async () =>
            {
                await Navigation.PushAsync(new NoteDetailPage(null));
            }));

            this.ToolbarItems.Add(new ToolbarItem("Log Out", null, async () =>
            {
                if (await DisplayAlert("Well?", "Are you sure you want to quit back to the login screen?", "Yep", "Nope"))
                {
                    await Navigation.PopAsync(true);
                }

            }));

        }

        /// <summary>
        /// Updates the note list view
        /// </summary>
        /// <returns></returns>
        private async Task PopulateNotesList()
        {
            await App.NoteService.GetAll().ContinueWith(task =>
            {
                var notes = task.Result;

                Notes = notes
                        .OrderByDescending(note => note.IsStarred)
                        .ThenByDescending(note => note.CreatedUtc)
                        .Select(s => new NoteListItemViewModel
                        {
                            NoteId = s.NoteId,
                            Title = s.Title,
                            StarImage = s.IsStarred ? "starred.png" : "notstarred.png"
                        })
                        .ToList();

                lvwNotes.ItemsSource = Notes;

                //CLear Item Slecteion
                lvwNotes.SelectedItem = null;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override async void OnAppearing()
        {
            await PopulateNotesList();
        }

        private void Lvw_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var note = e.SelectedItem as NoteListItemViewModel;
                Navigation.PushAsync(new NoteDetailPage(note.NoteId));
            }
        }

    }
}
