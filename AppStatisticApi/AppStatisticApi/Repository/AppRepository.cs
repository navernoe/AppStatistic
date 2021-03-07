using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AppStatisticApi.Storage;
using AppStatisticApi.Storage.Entities;
using AppStatisticApi.Exceptions;


namespace AppStatisticApi.Repository
{
    public class AppRepository
    {
        private AppStatisticDbContext db;

        public AppRepository(AppStatisticDbContext db)
        {
            this.db = db;
        }

        public async Task<ActionResult<AppEntity>> getById(int id)
        {

            AppEntity app = await db.Applications.FindAsync(id);

            return app;
        }

        public async Task<ActionResult<AppEntity>> create(AppEntity app)
        {
            try
            {
                db.Applications.Add(app);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                bool isUniqConstrainError = AppAlreadyExistsException.checkUniqConstrainError(e);

                if (isUniqConstrainError)
                {
                    throw new AppAlreadyExistsException($"Приложение {app.url} уже было добавлено", e);
                }
                else
                {
                    throw e;
                }
            }

            return app;
        }

        public async void update(AppEntity app, Dictionary<string, string> fieldValues)
        {
            Type appType = app.GetType();

            foreach (var pair in fieldValues)
            {
                string fieldName = pair.Key;
                string fieldValue = pair.Value;
                PropertyInfo appProp = appType.GetProperty(fieldName);

                if ( appProp != null )
                {
                    dynamic propValue = fieldValue;

                    if (appProp.PropertyType.FullName.Contains("System.Int64"))
                    {
                        Int64.TryParse(fieldValue, out long propLongValue);
                        propValue = propLongValue;
                    }

                    appProp.SetValue(app, propValue);
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
