using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InShapeModels;

namespace DAL
{
    public class ExpensesRepo
    {
        public static bool CreateExpense(StudioExpens studioExpens)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.StudioExpenses.Add(studioExpens);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static bool UpdateExpense(StudioExpens studioExpens)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.StudioExpenses.Attach(studioExpens);
                    context.Entry(studioExpens).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var expense = context.StudioExpenses.First(r => r.Id == id && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    expense.IsDeleted = true;
                    context.StudioExpenses.Attach(expense);
                    context.Entry(expense).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static ExpensesModel GetExpenseById(int id)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<ExpensesModel>(context.StudioExpenses.FirstOrDefault(x => x.Id == id));
            }
        }
    }
}
