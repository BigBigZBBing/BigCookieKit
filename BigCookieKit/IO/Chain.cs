using System;
using System.Collections.Generic;

namespace BigCookieKit.IO
{
    /// <summary>
    /// 根据节点索引获取节点的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node">当前节点</param>
    internal delegate void PostionByNode<T>(ChainNode<T> node) where T : class;

    /// <summary>
    /// 根据位置产生回调的委托
    /// </summary>
    /// <param name="curr"></param>
    internal delegate void CallByPostion(int position);

    /// <summary>
    /// 根据表达式获取节点的委托
    /// </summary>
    /// <param name="expression"></param>
    internal delegate void NodeByExpression<T>(Func<T, bool> expression, List<T> list) where T : class;

    /// <summary>
    /// 根据节点索引生成数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    internal delegate void ArrayByAllNode<T>(ref T[] array) where T : class;

    /// <summary>
    /// 链式流模型
    /// 劣势:
    /// 1.内存占用量大
    /// 2.最大长度小 暂时short.MaxLength没问题
    /// 3.写入速度慢
    /// 优势:
    /// 1.多样化查询快速 O(1)
    /// 2.删除快速 O(1)
    /// 3.转成数组快速 O(1)
    /// </summary>
    public class Chain<T>
        where T : class
    {
        /// <summary>
        /// 当前链
        /// </summary>
        internal ChainNode<T> m_chain;

        /// <summary>
        /// 链长度
        /// </summary>
        internal int m_length;

        /// <summary>
        /// 更新节点的坐标
        /// </summary>
        internal CallByPostion updateposition;

        /// <summary>
        /// 根据坐标通知所有节点
        /// </summary>
        internal CallByPostion informallnode;

        /// <summary>
        /// 刷新所有节点的尾部
        /// </summary>
        internal PostionByNode<T> refreshbottom;

        /// <summary>
        /// 根据表达式通知所有节点
        /// </summary>
        internal NodeByExpression<T> informallnodeexpression;

        /// <summary>
        /// 根据所有节点产生数组
        /// </summary>
        internal ArrayByAllNode<T> arrayallnode;

        /// <summary>
        /// 当前链对象
        /// </summary>
        public T Current { get => m_chain.m_item; }

        /// <summary>
        /// 链长度
        /// </summary>
        public int Length { get => m_length; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public int Position
        {
            get => m_chain.m_position;
            set
            {
                informallnode(value);
            }
        }

        /// <summary>
        /// 根据索引获取节点值
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                AtAssginNodeByPosition(index);
                return Current;
            }
            set
            {
                AtAssginNodeByPosition(index);
                m_chain.m_item = value;
            }
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var xnew = new ChainNode<T>();
            if (m_chain == null)
            {
                xnew.top = xnew;
                xnew.m_position = 0;
            }
            else
            {
                xnew.top = m_chain.top;
                xnew.prev = m_chain;
                m_chain.next = xnew;
                xnew.m_position = m_chain.m_position + 1;
            }
            xnew.m_item = item;
            xnew.callback += CompassCallBack;//回调当前节点
            updateposition += xnew.UpdatePosition;//更新节点在链式中的位置
            informallnode += xnew.CallBackByNode;//根据索引回调节点
            refreshbottom += xnew.RefreshBottom;//更新所有节点的末尾
            arrayallnode += xnew.ArrayByRefNode;//自动对位数组
            informallnodeexpression += xnew.CallBackByExpression;//根据表达式回调节点
            m_chain = xnew;
            refreshbottom(m_chain);
            m_length++;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            AtAssginNodeByPosition(index);
            m_chain.prev.next = m_chain.next;
            m_chain = null;
            m_length--;
            updateposition(index);
        }

        /// <summary>
        /// 根据表达式寻找节点
        /// </summary>
        /// <param name="expression"></param>
        public IEnumerable<T> Where(Func<T, bool> expression)
        {
            List<T> list = new List<T>();
            informallnodeexpression(expression, list);
            return list;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public bool ReadNext()
        {
            if (m_chain.next == null)
            {
                return false;
            }
            m_chain = m_chain.next;
            return true;
        }

        /// <summary>
        /// 转成数组
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] array = new T[m_length];
            arrayallnode(ref array);
            return array;
        }

        /// <summary>
        /// 根据位置分配节点
        /// </summary>
        private void AtAssginNodeByPosition(int index)
        {
            if (Position != index)
            {
                if (Position == index - 1)
                {
                    m_chain = m_chain.prev;
                }
                else if (Position == index + 1)
                {
                    m_chain = m_chain.next;
                }
                else if (Position == 0)
                {
                    m_chain = m_chain.top;
                }
                else if (Position == m_length)
                {
                    m_chain = m_chain.bottom;
                }
                else
                {
                    informallnode(index);
                }
            }
        }

        /// <summary>
        /// 通过回调更新节点
        /// </summary>
        /// <param name="currnode"></param>
        private void CompassCallBack(ChainNode<T> currnode)
        {
            m_chain = currnode;
        }

    }

    /// <summary>
    /// 内部节点
    /// </summary>
    internal class ChainNode<T>
        where T : class
    {
        /// <summary>
        /// 指向回调
        /// </summary>
        internal PostionByNode<T> callback;

        /// <summary>
        /// 节点对象
        /// </summary>
        internal T m_item;

        /// <summary>
        /// 节点索引
        /// </summary>
        internal int m_position;

        /// <summary>
        /// 上一个
        /// 方便倒序查询
        /// </summary>
        internal ChainNode<T> prev;

        /// <summary>
        /// 下一个
        /// 方便顺序查询
        /// </summary>
        internal ChainNode<T> next;

        /// <summary>
        /// 链头
        /// </summary>
        internal ChainNode<T> top;

        /// <summary>
        /// 链尾
        /// 方便尾部插入
        /// </summary>
        internal ChainNode<T> bottom;

        /// <summary>
        /// 伸缩索引
        /// </summary>
        /// <param name="position"></param>
        internal void UpdatePosition(int position)
        {
            if (this.m_position > position)
                this.m_position--;
        }

        /// <summary>
        /// 根据索引回调节点
        /// </summary>
        /// <param name="position"></param>
        internal async void CallBackByNode(int position)
        {
            if (this.m_position == position)
            {
                callback(this);
            }
        }

        /// <summary>
        /// 根据表达式回调节点
        /// </summary>
        /// <param name="expression"></param>
        internal async void CallBackByExpression(Func<T, bool> expression, List<T> list)
        {
            if (expression(this.m_item))
            {
                list.Add(this.m_item);
            }
        }

        /// <summary>
        /// 尾部指针回调
        /// </summary>
        /// <param name="currnode"></param>
        internal async void RefreshBottom(ChainNode<T> currnode)
        {
            this.bottom = currnode;
        }

        /// <summary>
        /// 根据索引给数组相应位置赋值
        /// </summary>
        /// <param name="array"></param>
        internal void ArrayByRefNode(ref T[] array)
        {
            array[m_position] = m_item;
        }
    }
}
