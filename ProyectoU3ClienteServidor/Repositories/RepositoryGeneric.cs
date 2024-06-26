﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoU3ClienteServidor.Repositories
{
    public class RepositoryGeneric<T> where T : class, new()
    {
        SQLiteConnection context;

        public RepositoryGeneric()
        {
            string ruta = FileSystem.AppDataDirectory + "/actividades.db3";
            context = new SQLiteConnection(ruta);
            context.CreateTable<T>();
        }

        public void InsertOrReplace(T item)
        {
            context.InsertOrReplace(item);
        }

        public IEnumerable<T> GetAll()
        {
            return context.Table<T>().ToList();
        }

        public T? Get(int id)
        {
            return context.Find<T>(id);
        }

        public void Insert(T item)
        {
            context.Insert(item);
        }

        public void Update(T item)
        {
            context.Update(item);
        }

        public void Delete(T item)
        {
            context.Delete(item);
        }

        public void DeleteAll()
        {
            var allItems = GetAll();
            foreach (var item in allItems)
            {
                context.Delete(item);
            }
        }
    }
}

