﻿using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Operations.Command;

// syntax:
// INSERT INTO <Class>|cluster:<cluster>|index:<index> 
// [<cluster>](cluster) 
// [VALUES (<expression>[,]((<field>[,]*))*)]|[<field> = <expression>[,](SET)*]

namespace Orient.Client
{
    public class OSqlInsert
    {
        private SqlQuery _sqlQuery = new SqlQuery();
        private Connection _connection;

        public OSqlInsert()
        {
        }

        internal OSqlInsert(Connection connection)
        {
            _connection = connection;
        }

        #region Insert

        public OSqlInsert Insert(string className)
        {
            return Into(className);
        }

        public OSqlInsert Insert<T>()
        {
            return Into<T>();
        }

        public OSqlInsert Insert<T>(T obj)
        {
            // check for OClassName shouldn't have be here since INTO clause might specify it

            _sqlQuery.Insert(obj);

            return this;
        }

        #endregion

        #region Into

        public OSqlInsert Into(string className)
        {
            _sqlQuery.Class(className);

            return this;
        }

        public OSqlInsert Into<T>()
        {
            Into(typeof(T).Name);

            return this;
        }

        #endregion

        #region Cluster

        public OSqlInsert Cluster(string clusterName)
        {
            _sqlQuery.Cluster(clusterName);

            return this;
        }

        public OSqlInsert Cluster<T>()
        {
            return Cluster(typeof(T).Name);
        }

        #endregion

        #region Set

        public OSqlInsert Set<T>(string fieldName, T fieldValue)
        {
            _sqlQuery.Set<T>(fieldName, fieldValue);

            return this;
        }

        public OSqlInsert Set<T>(T obj)
        {
            _sqlQuery.Set(obj);

            return this;
        }

        #endregion

        #region Run

        public ODocument Run()
        {
            CommandPayloadCommand payload = new CommandPayloadCommand();
            payload.Text = ToString();

            Command operation = new Command(_connection.Database);
            operation.OperationMode = OperationMode.Synchronous;
            operation.CommandPayload = payload;

            OCommandResult result = new OCommandResult(_connection.ExecuteOperation(operation));

            return result.ToSingle();
        }

        public T Run<T>() where T : class, new() 
        {
            return Run().To<T>();
        }

        #endregion

        public override string ToString()
        {
            return _sqlQuery.ToString(QueryType.Insert);
        }
    }
}
