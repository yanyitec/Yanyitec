using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace Yanyitec.Data
{
    public class DbCommandFactory<TEnitity>
    {
        public DbCommandFactory(EntityInfo info, DbCommandBuilderFactory builderFactory) {
            this.EntityInfo = info;
            this.DbCommandBuilderFactory = builderFactory;
        }
        public EntityInfo EntityInfo { get; private set; }
        public DbCommandBuilderFactory DbCommandBuilderFactory { get; private set; }
        Action<IDbCommand,TEnitity> _insert;
        Action<IDbCommand, TEnitity> _update;
        SelectInfo<TEnitity> _select;
        readonly SortedDictionary<string, Action<IDbCommand, TEnitity>> _inserts = new SortedDictionary<string, Action<IDbCommand, TEnitity>>();
        readonly SortedDictionary<string, Action<IDbCommand, TEnitity>> _updates = new SortedDictionary<string, Action<IDbCommand, TEnitity>>();
        readonly SortedDictionary<string, SelectInfo<TEnitity>> _selects = new SortedDictionary<string, SelectInfo<TEnitity>>();

        readonly System.Threading.ReaderWriterLockSlim _locker = new System.Threading.ReaderWriterLockSlim();

        public void BuildInsert(IDbCommand cmd,TEnitity entity, string fields = null) {
            if (fields == null) {
                if (_insert == null)
                {
                    lock (_locker)
                    {
                        if (_insert == null) _insert = this.DbCommandBuilderFactory.GenInsertBuilder<TEnitity>(this.EntityInfo, null);
                    }
                }
                
                _insert(cmd, entity);
                return;
            }
            Action<IDbCommand, TEnitity> func = null;
            _locker.EnterUpgradeableReadLock();
            try
            {
                if (!_inserts.TryGetValue(fields, out func))
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        if (!_inserts.TryGetValue(fields, out func))
                        {
                            func = this.DbCommandBuilderFactory.GenInsertBuilder<TEnitity>(this.EntityInfo,fields);
                            _inserts.Add(fields, func);
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
            func(cmd, entity);
        }

        public void BuildUpdate(IDbCommand cmd, TEnitity entity, string fields = null)
        {
            if (fields == null)
            {
                if (_update == null) {
                    lock (_locker)
                    {
                        if (_update == null) _update = this.DbCommandBuilderFactory.GenUpdateBuilder<TEnitity>(this.EntityInfo, null);
                    }
                }
                
                _update(cmd, entity);
                return;
            }
            Action<IDbCommand, TEnitity> func = null;
            _locker.EnterUpgradeableReadLock();
            try
            {
                if (!_updates.TryGetValue(fields, out func))
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        if (!_updates.TryGetValue(fields, out func))
                        {
                            func = this.DbCommandBuilderFactory.GenUpdateBuilder<TEnitity>(this.EntityInfo, fields);
                            _updates.Add(fields, func);
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
            func(cmd, entity);
        }

        public SelectInfo<TEnitity> GetOrCreateSelect(string fields = null,string tbAlias=null)
        {
            if (fields == null)
            {
                if (_select == null)
                {
                    lock (_locker)
                    {
                        if (_select == null) _select = this.DbCommandBuilderFactory.GenSelectInfo<TEnitity>(this.EntityInfo,fields,tbAlias);
                    }
                }

                
                return _select;
            }
            SelectInfo<TEnitity> result = null;
            _locker.EnterUpgradeableReadLock();
            try
            {
                if (!_selects.TryGetValue(fields, out result))
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        if (!_selects.TryGetValue(fields, out result))
                        {
                            result = this.DbCommandBuilderFactory.GenSelectInfo<TEnitity>(this.EntityInfo, fields,tbAlias);
                            _selects.Add(fields, result);
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
            return result;
        }
        

        //public Action

    }
}
