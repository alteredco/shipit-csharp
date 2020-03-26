using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Npgsql;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;

namespace ShipIt.Repositories
{
    public interface IEmployeeRepository
    {
        int GetCount();
        int GetWarehouseCount();
        [Obsolete("This property is obsolete. Use GetEmployeeByEmployeeId or GetEmployeesByName instead.", false)]
        EmployeeDataModel GetEmployeeByName(string name);
        IEnumerable<EmployeeDataModel> GetEmployeesByName(string name);
        EmployeeDataModel GetEmployeeByEmployeeId(int employeeId);
        IEnumerable<EmployeeDataModel> GetEmployeesByWarehouseId(int warehouseId);
        EmployeeDataModel GetOperationsManager(int warehouseId);
        void AddEmployees(IEnumerable<Employee> employees);
        [Obsolete("This property is obsolete. Use RemoveEmployeeByEmployeeId instead.", false)]
        void RemoveEmployee(string name);

        void RemoveEmployeeByEmployeeId(int employeeId);
    }

    public class EmployeeRepository : RepositoryBase, IEmployeeRepository
    {
        public static IDbConnection CreateSqlConnection()
        {
            return new NpgsqlConnection(ConnectionHelper.GetConnectionString());
        }

        public int GetCount()
        {

            using (IDbConnection connection = CreateSqlConnection())
            {
                var command = connection.CreateCommand();
                string EmployeeCountSQL = "SELECT COUNT(*) FROM em";
                command.CommandText = EmployeeCountSQL;
                connection.Open();
                var reader = command.ExecuteReader();

                try
                {
                    reader.Read();
                    return (int) reader.GetInt64(0);
                }
                finally
                {
                    reader.Close();
                }
            };
        }

        public int GetWarehouseCount()
        {
            using (IDbConnection connection = CreateSqlConnection())
            {
                var command = connection.CreateCommand();
                string EmployeeCountSQL = "SELECT COUNT(DISTINCT w_id) FROM em";
                command.CommandText = EmployeeCountSQL;
                connection.Open();
                var reader = command.ExecuteReader();

                try
                {
                    reader.Read();
                    return (int)reader.GetInt64(0);
                }
                finally
                {
                    reader.Close();
                }
            };
        }
        
        public EmployeeDataModel GetEmployeeByName(string name)
        {
            string sql = "SELECT name, w_id, role, ext FROM em WHERE name = @name";
            var parameter = new NpgsqlParameter("@name", name);
            string noProductWithIdErrorMessage = string.Format("No employees found with name: {0}", name);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader),noProductWithIdErrorMessage, parameter);
        }
        
        public IEnumerable<EmployeeDataModel> GetEmployeesByName(string name)
        {
            string sql = "SELECT em_id, name, w_id, role, ext FROM em WHERE name = @name";
            var parameter = new NpgsqlParameter("@name", name);
            string errorMessage = string.Format("No employees found with name: {0}", name);
            return base.RunGetQuery(sql, reader => new EmployeeDataModel(reader),errorMessage, parameter);
        }
        
        public EmployeeDataModel GetEmployeeByEmployeeId(int employeeId)
        {
            string sql = "SELECT em_id, name, w_id, role, ext FROM em WHERE em_id = @em_id";
            var parameter = new NpgsqlParameter("@em_id", employeeId);
            string errorMessage = string.Format("No employees found with id: {0}", employeeId);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader),errorMessage, parameter);
        }

        public IEnumerable<EmployeeDataModel> GetEmployeesByWarehouseId(int warehouseId)
        {

            string sql = "SELECT em_id, name, w_id, role, ext FROM em WHERE w_id = @w_id";
            var parameter = new NpgsqlParameter("@w_id", warehouseId);
            string errorMessage =
                string.Format("No employees found with Warehouse Id: {0}", warehouseId);
            return base.RunGetQuery(sql, reader => new EmployeeDataModel(reader), errorMessage, parameter);
        }

        public EmployeeDataModel GetOperationsManager(int warehouseId)
        {

            string sql = "SELECT name, w_id, role, ext FROM em WHERE w_id = @w_id AND role = @role";
            var parameters = new []
            {
                new NpgsqlParameter("@w_id", warehouseId),
                new NpgsqlParameter("@role", DataBaseRoles.OperationsManager)
            };

            string noProductWithIdErrorMessage =
                string.Format("No employees found with Warehouse Id: {0}", warehouseId);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader), noProductWithIdErrorMessage, parameters);
        }

        public void AddEmployees(IEnumerable<Employee> employees)
        {
            string sql = "INSERT INTO em (name, w_id, role, ext) VALUES(@name, @w_id, @role, @ext)";
            
            var parametersList = new List<NpgsqlParameter[]>();
            foreach (var employee in employees)
            {
                var employeeDataModel = new EmployeeDataModel(employee);
                parametersList.Add(employeeDataModel.GetNpgsqlParameters().ToArray());
            }

            base.RunTransaction(sql, parametersList);
        }

        public void RemoveEmployee(string name)
        {
            string sql = "DELETE FROM em WHERE name = @name";
            var parameter = new NpgsqlParameter("@name", name);
            var rowsDeleted = RunSingleQueryAndReturnRecordsAffected(sql, parameter);
            if (rowsDeleted == 0)
            {
                throw new NoSuchEntityException("Incorrect result size: expected 1, actual 0");
            }
            else if (rowsDeleted > 1)
            {
                throw new InvalidStateException("Unexpectedly deleted " + rowsDeleted + " rows, but expected a single update");
            }
        }
        
        public void RemoveEmployeeByEmployeeId(int employeeId)
        {
            string sql = "DELETE FROM em WHERE em_id = @em_id";
            var parameter = new NpgsqlParameter("@em_id", employeeId);
            var rowsDeleted = RunSingleQueryAndReturnRecordsAffected(sql, parameter);
            if (rowsDeleted == 0)
            {
                throw new NoSuchEntityException("Incorrect result size: expected 1, actual 0");
            }
            else if (rowsDeleted > 1)
            {
                throw new InvalidStateException("Unexpectedly deleted " + rowsDeleted + " rows, but expected a single update");
            }
        }
    }
}