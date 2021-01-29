using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WFlow
{
    public class ErrorLogger:IErrorLogger
    {
        readonly string baseDir;
        public ErrorLogger(string baseDir = null) {
            this.baseDir = baseDir;
            if (string.IsNullOrWhiteSpace(baseDir)) {
                this.baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"wf_errors");
            }
        }

        public void Log(Engine flow,Activity activity,ActivityEntity entity, Exception ex)
        {
            var dir = this.baseDir;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (flow != null)
            {
                var flowName = flow.Name;
                if (string.IsNullOrWhiteSpace(flowName)) flowName = "--DFT-FLOW-NAME--";
                dir = Path.Combine(dir, flowName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            else {
                dir = Path.Combine(dir, "--ERRORS--");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            
            string filename;
            var now = DateTime.Now;
            if (entity != null)
            {
                dir = Path.Combine(dir, entity.FlowId.ToString());
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                filename = Path.Combine(dir,entity.Id.ToString() + "." + now.ToString("yyyyMMddhhmmss") + "." + now.Millisecond.ToString() +".txt");
            }
            else {
                dir = Path.Combine(dir, "--ERRORS--");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var date = now.ToString("yyyyMMdd-hh");
                dir = Path.Combine(dir, date);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                filename = Path.Combine(dir, entity.Id.ToString() + "." + now.ToString("mmss") + "." + now.Millisecond.ToString() +"." + Guid.NewGuid().ToString() +".txt");
            }
            WriteToFile(filename,activity,entity,ex);
            
            
        }

        static void WriteToFile(string filename, Activity activity, ActivityEntity entity, Exception ex) {
            using var stream = new StreamWriter(filename, true);
            stream.WriteLine("==MESSAGE==");
            stream.WriteLine(ex?.Message);
            stream.WriteLine();
            if (ex != null) {
                while (ex != null) {
                    stream.WriteLine("==EXCEPTION==");
                    stream.WriteLine(ex.Message);
                    stream.WriteLine(ex.StackTrace);
                    stream.WriteLine();
                    ex = ex.InnerException;
                }
                
            }
            stream.WriteLine("==ENTITY==");
            stream.WriteLine(JSON.Stringify(entity));
            stream.WriteLine();

            stream.WriteLine("==STATUS==");
            stream.WriteLine(JSON.Stringify(activity?.Status.ToString()));
            stream.WriteLine();

            stream.WriteLine("==STATES==");
            stream.WriteLine(JSON.Stringify(activity?.States));
            stream.WriteLine();

            stream.WriteLine("==RESULTS==");
            stream.WriteLine(JSON.Stringify(activity?.Results));
            stream.WriteLine();

        }

        public static IErrorLogger Default = new ErrorLogger();
    }
}
