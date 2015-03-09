using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using EPiServer.Data;
using EPiServer.DataAccess;
using log4net;

namespace BVNetwork.EPiSendMail.DataAccess.DataUtil
{
    /// <summary>
    /// Data access functions for getting data to and from the database
    /// </summary>
    public class SystemData : DataAccessBase
    {
        public SystemData(IDatabaseHandler databaseHandler)
            : base(databaseHandler)
        {
            this.Database = databaseHandler;
        }

        /// <summary>
        /// We cannot use a static logger here as the base class defines that
        /// </summary>
        /// <returns></returns>
        protected ILog GetLogger()
        {
            return LogManager.GetLogger(GetType());
        }

        public int GetNewsletterDatabaseVersion()
        {

            int version = 0;

            Database.Execute(() =>
                                      {
                                          DbCommand cmd = CreateCommand("NewsletterDatabaseVersion");
                                          DbParameter parameter = CreateParameter("@ReturnVal", DbType.Int32);
                                          parameter.Direction = ParameterDirection.ReturnValue;
                                          cmd.Parameters.Add(parameter);

                                          cmd.ExecuteNonQuery();
                                          var result = parameter.Value;

                                          if (result == null)
                                          {
                                              GetLogger().Warn("Unable to get database version for module.");
                                              throw new ApplicationException(
                                                  "Unable to get database version for module.");
                                          }
                                          if (int.TryParse(result.ToString(), out version) == false)
                                          {
                                              GetLogger()
                                                  .Warn(
                                                      "Unable to get database version for module. Unexpected return value: " +
                                                      result);
                                              throw new ApplicationException(
                                                  "Unable to get database version for module. Unexpected return value: " + result);
                                          }
                                      });

            GetLogger().DebugFormat("Current Newsletter Database Version is {0}", version);
            return version;

        }

        public void SetNewsletterDatabaseVersion(int version)
        {

            Database.Execute(() =>
            {
                string sqlCommand =
                    @"if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[NewsletterDatabaseVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].NewsletterDatabaseVersion
";
                DbCommand cmd = this.CreateTextCommand(sqlCommand);

                cmd.ExecuteNonQuery();
            });

            Database.Execute(() =>
            {
                string sqlCommand =
                    @"CREATE PROCEDURE [dbo].[NewsletterDatabaseVersion]
    AS
	RETURN " + version.ToString();

                DbCommand cmd = this.CreateTextCommand(sqlCommand);

                cmd.ExecuteNonQuery();
            });

        }


        public void InstallNewsletterDatabase(int version)
        {
            // Read command text
            string script = GetResourceContent(version);
            if (script == null) 
                throw new ArgumentNullException("Cannot find installation script for " + version.ToString());

            //split the script on "GO" commands (note must not be whitespace before or after GO
            // NOTE! This Regex is probably better (.*?\n\s*go\s*\n)(?!\s*\*\/)
            string[] splitter = { "\r\nGO\r\n" };
            string[] commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            Database.Execute(() =>
                {
                    foreach (string commandText in commandTexts)
                    {
                        GetLogger().Debug("Running sql: \r\n" + commandText);
                        DbCommand cmd = CreateTextCommand(commandText);
                        cmd.ExecuteNonQuery();
                    }
                });

        }


        public string GetResourceContent(int version)
        {
            string resourceName = "BVNetwork.EPiSendMail.Tools.newsletter-install-" + version.ToString() + ".sql";
            Assembly assembly = Assembly.GetExecutingAssembly();
            string result = null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }                
            }
            return result;
        }

    }
}
