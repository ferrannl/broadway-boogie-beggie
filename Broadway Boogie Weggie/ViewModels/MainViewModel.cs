﻿using Broadway_Boogie_Weggie.Builders;
using Broadway_Boogie_Weggie.Commands;
using Broadway_Boogie_Weggie.Factories;
using Broadway_Boogie_Weggie.Importers;
using Broadway_Boogie_Weggie.Models;
using Broadway_Boogie_Weggie.Parsers;
using Broadway_Boogie_Weggie.Readers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Broadway_Boogie_Weggie.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand SetupGalleryDiscCommand { get; set; }
        public ObservableCollection<Tile> ObservableTiles { get; set; }
        public ObservableCollection<Artist> ObservableArtists { get; set; }

        private GalleryBuilder builder = null;
        private Gallery gallery = null;

        public MainViewModel()
        {
            ObservableTiles = new ObservableCollection<Tile>();
            ObservableArtists = new ObservableCollection<Artist>();
            SetupGalleryDiscCommand = new SetupGalleryDiscCommand();
            CompositionTarget.Rendering += (s, e) => UpdateGallery();
        }

        public void SetupGallery(string importType)
        {
            try
            {
                ObservableTiles.Clear();
                ObservableArtists.Clear();
                string filePath = Import(importType);
                List<string> content = Read(filePath);
                gallery = null;
                if (content[0].Equals("csv"))
                {
                    gallery = Build(CsvParse(content));
                }
                else if (content[0].Equals("xml"))
                {
                    gallery = Build(XmlParse(content));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void UpdateGallery()
        {
            if (gallery != null)
            {
                gallery.Tick();
                gallery.Draw();
            }
        }

        private List<string> Read(string filePath)
        {
            IReader reader = ReaderFactory.Instance.CreateReader(filePath);
            return reader.Read(filePath);
        }

        private string Import(string importType)
        {
            Importer importer = ImporterFactory.Instance.CreateImporter(importType);
            return importer.Import();
        }

        private List<Tile> XmlParse(List<string> content)
        {
            XmlParser parser = ParserFactory.Instance.CreateXmlParser(content[0]);
            return parser.Parse(content);
        }
        private List<Artist> CsvParse(List<string> content)
        {
            CsvParser parser = ParserFactory.Instance.CreateCsvParser(content[0]);
            return parser.Parse(content);
        }
        private Gallery Build(List<Tile> tileList)
        {
            if (builder == null)
            {
                builder = new GalleryBuilder();
            }
            foreach (Tile t in tileList)
            {
                builder.AddTile(t);
            }
            return builder.Build();
        }
        private Gallery Build(List<Artist> artistList)
        {
            if (builder == null)
            {
                builder = new GalleryBuilder();
            }
            foreach (Artist a in artistList)
            {
                builder.AddArtist(a);
            }
            return builder.Build();
        }

    }
}

