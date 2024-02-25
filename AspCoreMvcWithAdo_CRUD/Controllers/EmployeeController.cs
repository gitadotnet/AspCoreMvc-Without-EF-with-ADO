using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using AspCoreMvcWithAdo_CRUD.Models;

namespace AspCoreMvcWithAdo_CRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;   
        public EmployeeController(IConfiguration configuration) {
            _configuration = configuration;
        }

        // GET: Employee
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DemoDBConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("usp_get_employee", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

       
        // GET: Employee/AddEdit/5
        public IActionResult AddEdit(int? id)
        {
            Employee employee = new Employee();
            if (id > 0)
                employee = FetchEmployeeByID(id);
            return View(employee);
        }

        // POST: Employee/AddEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(int id, [Bind("ID,Name,Age,IsActive")] Employee employee)
        {
            if (id != employee.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DemoDBConnection")))
                    {
                        sqlConnection.Open();
                        SqlCommand sqlCmd = new SqlCommand("usp_AddEdit_employee", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("ID", employee.ID);
                        sqlCmd.Parameters.AddWithValue("Name", employee.Name);
                        sqlCmd.Parameters.AddWithValue("Age", employee.Age);
                        sqlCmd.Parameters.AddWithValue("IsActive", employee.IsActive);
                        sqlCmd.ExecuteNonQuery();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                        throw;
                }
            }
            return View(employee);
        }
        

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DemoDBConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("usp_delete_employee", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("ID", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }
        [NonAction]
        public Employee FetchEmployeeByID(int? id)
        {
            Employee employee = new Employee();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DemoDBConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("usp_get_employee_by_id", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("ID", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    employee.ID = Convert.ToInt32(dtbl.Rows[0]["ID"].ToString());
                    employee.Name = dtbl.Rows[0]["Name"].ToString();
                    employee.Age = Convert.ToInt32(dtbl.Rows[0]["Age"]);
                    employee.IsActive = Convert.ToBoolean(dtbl.Rows[0]["IsActive"]);
                }
                return employee;
            }
        }
    }
}
