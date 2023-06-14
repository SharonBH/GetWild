using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using InShapeModels;
using Utilities;

namespace BLL
{
    public class ExpensesBLL
    {
        public static bool Create(ExpensesModel model)
        {
            //model.StudioId = StudioBLL.GetDefaultStudioByComapny();
            return ExpensesRepo.CreateExpense(Mapper.Map<StudioExpens>(model));
        }

        public static bool Update(ExpensesModel model)
        {
            return ExpensesRepo.UpdateExpense(Mapper.Map<StudioExpens>(model));
        }

        public static bool Delete(int Id)
        {
            return ExpensesRepo.Delete(Id);
        }

        public static ExpensesModel GetById(int id)
        {
            return ExpensesRepo.GetExpenseById(id);
        }
    }
}
