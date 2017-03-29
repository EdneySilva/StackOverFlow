using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InjetandoInteracaoComUsuario.Controllers
{
    public interface IQuestionBox
    {
        string Id { get; }
        Task<bool> ShowYesNoQuestionBox(string question);
        HtmlString ShowMessage();
    }

    class QuestionBox : IQuestionBox
    {
        Process CurrentProcess { get; set; }

        public string Id { get; } = Guid.NewGuid().ToString();
        private string Question { get; set; }

        public QuestionBox(Process currentProcess)
        {
            CurrentProcess = currentProcess;
            CurrentProcess.RegisterForAnswer(this.Id);
        }

        public Task<bool> ShowYesNoQuestionBox(string question)
        {
            Question = question;
            CurrentProcess.Release();
            return AwaitForAnswer();
        }

        public HtmlString ShowMessage()
        {
            HtmlString htm = new HtmlString(
                $"<script>showMessage('{Question}', '{Id}');</script>"
            );

            return htm;
        }

        private Task<bool> AwaitForAnswer()
        {
            TaskCompletionSource<bool> awaitableResult = new TaskCompletionSource<bool>(true);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    var answare = CurrentProcess.GetAnswer(this.Id);
                    if (!answare.HasAnswered)
                        continue;
                    awaitableResult.SetResult(answare.Value);
                    break;
                }
            });

            return awaitableResult.Task;
        }
    }

    public class SaleService
    {
        public async Task<string> GetValue(IQuestionBox qbox)
        {
            if (await qbox.ShowYesNoQuestionBox("Do you think Edney is the big guy ?"))
            {
                return "I knew, Edney is the big guy";
            }
            return "No I disagree";
        }
    }

    public class Answer
    {
        public static Answer Empty { get; } = new Answer { Value = true, HasAnswered = true };

        public Answer()
        {

        }

        public Answer(TaskCompletionSource<object> completedTask)
        {
            CompletedTask = completedTask;
        }

        private TaskCompletionSource<object> CompletedTask { get; set; }

        public bool Value { get; set; }
        public bool HasAnswered { get; set; }

        public void UserResponse(object response)
        {
            CompletedTask.SetResult(response);
        }
    }

    public class Process
    {
        private static Dictionary<string, Answer> StatusReport = new Dictionary<string, Answer>();

        TaskCompletionSource<bool> AwaitableResult { get; } = new TaskCompletionSource<bool>(true);

        IQuestionBox QuestionBox { get; set; }
        public IQuestionBox Run(Action<IQuestionBox> action)
        {
            QuestionBox = new QuestionBox(this);
            Task.Factory.StartNew(() =>
            {
                action(QuestionBox);
            });
            Task.WaitAll(AwaitableResult.Task);
            return QuestionBox;
            //return tsc.Task;
        }

        public void RegisterForAnswer(string id)
        {
            if (StatusReport.ContainsKey(id))
                return;
            StatusReport.Add(id, new Answer()
            {
            });
        }

        public Answer GetAnswer(string id)
        {
            if (!StatusReport.ContainsKey(id))
                return Answer.Empty;
            return StatusReport[id];
        }

        public void Release()
        {
            AwaitableResult.SetResult(true);
        }

        public void End(object userResponse)
        {
            if (!StatusReport.ContainsKey(QuestionBox.Id))
                return;
            StatusReport[QuestionBox.Id].UserResponse(userResponse);
        }

        public static Task<object> DefineAnswer(string id, bool result)
        {
            if (!StatusReport.ContainsKey(id))
                return Task.FromResult((object)"Success on the operation");
            TaskCompletionSource<object> completedTask = new TaskCompletionSource<object>();
            StatusReport[id] = new Answer(completedTask)
            {
                HasAnswered = true,
                Value = result
            };
            return completedTask.Task;
        }
    }

}