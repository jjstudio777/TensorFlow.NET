/*****************************************************************************
   Copyright 2020 The TensorFlow.NET Authors. All Rights Reserved.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
******************************************************************************/
#define MULTI_THREAD_RUN

using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Tensorflow.Contexts
{
    /// <summary>
    /// Match the semantics of DefaultGraphStack
    /// </summary>
    public class ContextSwitchStack
    {
#if MULTI_THREAD_RUN
        ConcurrentDictionary<int, Stack<ContextSwitch>> stack_per_thread = new ConcurrentDictionary<int, Stack<ContextSwitch>>();
        Stack<ContextSwitch> stack
        {
            get
            {
                return stack_per_thread.GetOrAdd(System.Threading.Thread.CurrentThread.ManagedThreadId, new Stack<ContextSwitch>());
            }
        }
#else
        Stack<ContextSwitch> stack;
#endif

        public ContextSwitchStack(bool isEager, bool isFunc)
        {
#if MULTI_THREAD_RUN
#else
            stack = new Stack<ContextSwitch>();
#endif
            Push(isEager, isFunc);
        }

        public void Push(bool isEager, bool isFunc)
        {
            stack.Push(new ContextSwitch
            {
                EagerMode = isEager,
                IsBuildingFunction = isFunc
            });
        }

        public void Clear()
        {
            stack.Clear();
        }

        public void Pop()
        {
            stack.Pop();
        }

        public int Count()
        {
            return stack.Count;
        }

        public ContextSwitch Current()
        {
            return stack.Peek();
        }
    }
}
