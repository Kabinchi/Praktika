    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.EntityFrameworkCore;

    namespace Airport.Other
    {
        public static class DataOperations
        {
            public static void DeleteRow<T>(MyDbContext context, int id, Action reloadMethod) where T : class
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var entity = context.Set<T>().Find(id);
                        if (entity != null)
                        {
                            context.Set<T>().Remove(entity);
                            context.SaveChanges();
                            reloadMethod?.Invoke();
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException && (sqlException.Number == 547 || sqlException.Number == 2601))
                        {
                            MessageBox.Show("На эту запись ссылаются другие записи. Удаление невозможно.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }
