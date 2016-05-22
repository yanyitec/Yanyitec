using System;
using Yanyitec.Storaging;
using Yanyitec.Workflow.Definations;

namespace Yanyitec.Workflow
{
    public interface IRepository
    {
        IStorage DefinationStorage { get; }
        IStorage RuntimeStorage { get; }

        Guid CreateProcess(Guid activityDefinationId, string description);
        Defination GetActivity(Guid runtimeId);
        Defination GetActivity(Guid runtimeId, Guid processId);
        Defination GetDefination(Guid id);
        bool SaveActivity(Defination data);
        void SaveDefination(Defination defination);
        void SaveProcess(ProcessDefination data);
    }
}