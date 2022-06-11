/**
 * JDialog是一个简单易用但是功能强大的开源JS弹出窗口，具有很强的扩展性和兼容性，兼容IE6.0，目前版本1.2.
 * 包括锁屏对象JDialog.lock， 提示工具 JDialog.tip， 确认框 JDialog.confirm  弹出窗口 JDialog.win 比artDialog功能更强大，使用更方便。
 * url : https://git.oschina.net/blackfox/JDialog
 * @dependence jquery
 * @author  yangjian102621@gmail.com
 * @version 2.3
 */

(function ($) {

    /**
     * 生成一个 DOM 元素的 id
     * @param prefix id 前缀
     */
    function generateElementId(prefix) {
        return (prefix + "_" + Math.random()).replace("0.", "");
    }

    //获取页面高度
    function getPageHeight() {
        return Math.max(document.documentElement.clientHeight, document.body.clientHeight, $(document).height()) + document.documentElement.scrollTop;
    }
    //获取窗口高度
    function getWindowHeight() {
        return $(window).height();
    }
    //获取窗口宽度
    function getWindowWidth() {
        return $(window).width();
    }

    //获取滚动条的高度
    function getScrollTop() {
        return window.document.body.scrollTop || window.document.documentElement.scrollTop;
    }

    //判断变量是否是数组
    function isArray(vars) {
        return Object.prototype.toString.apply(vars) === "[object Array]"
    }

    //判断变量是否是对象
    function isObject(vars) {
        return Object.prototype.toString.apply(vars) === "[object Object]"
    }

    /**
     * 根据对齐方式设置元素的位置
     * @param width 元素的宽度
     * @param height 元素的高度
     * @param offset 位置, 可选项[cc, lt, tc, rt, lc, rc, lb, bc, rb], 或者直接是一个数组 [top, left] || {top:'', left:''}
     * @param container
     */
    $.fn.setPosition = function (width, height, offset, container) {
        //计算元素的位置
        var top, left;
        // 处理容器相对位置，默认容器是当前窗口
        var x0 = 0, y0 = 0, cwidth = getWindowWidth(), cheight = getWindowHeight();
        if (container) { //相对某个容器对齐
            var __offset = $(container).offset();
            x0 = __offset.left;
            y0 = __offset.top;
            cwidth = $(container).width();
            cheight = $(container).height();
        } else {
            y0 = getScrollTop();
        }

        switch (offset) {

            case 'cc': //中部居中
                top = y0 + (cheight - height) / 2;
                left = x0 + (cwidth - width) / 2;
                break;
            case 'lt': //左上
                top = y0 + JDialog.margin;
                left = JDialog.margin + x0;
                break;
            case 'tc': //顶部居中
                top = y0 + JDialog.margin;
                left = x0 + (cwidth - width) / 2;
                break;
            case 'rt': //右上
                top = y0 + JDialog.margin;
                left = x0 + cwidth - width - JDialog.margin;
                break;
            case 'lc': //中部居左
                top = y0 + (cheight - height) / 2;
                left = x0 + JDialog.margin;
                break;
            case 'rc': //中部居右
                top = y0 + (cheight - height) / 2;
                left = x0 + cwidth - width - JDialog.margin;
                break;
            case 'lb': //左下
                top = y0 + cheight - height - JDialog.margin;
                left = x0 + JDialog.margin;
                break;
            case 'rb': //右下
                top = y0 + cheight - height - JDialog.margin;
                left = x0 + cwidth - width - JDialog.margin;
                break;
            case 'bc': //底部居中
                top = y0 + cheight - height - JDialog.margin;
                left = x0 + (cwidth - width) / 2;
                break;
            default:
                if (typeof offset == 'number') {
                    top = y0 + offset;
                    left = x0 + (cwidth - width) / 2;
                } else if (isObject(offset)) {
                    top = y0 + offset.top;
                    left = x0 + offset.left;
                } else if (isArray(offset)) {
                    top = y0 + offset[0];
                    left = x0 + offset[1];
                }
        }

        $(this).css({
            'width': width + 'px',
            'height': height + 'px',
            'top': top + 'px',
            'left': left + 'px'
        });

        return this;
    }

    /********************************************************************
     * 锁屏对象
     * ******************************************************************
     * @param options 选项
     */
    var __locker__ = function (options) {
        var exports = {};
        var $lock = null;   //锁屏的 dom 对象
        //创建元素
        function create() {
            if ($lock != null) return;
            $lock = $('<div id="' + options.id + '" class="lock_panel"></div>');
            $lock.css({ 'background-color': options.bgcolor });
            $('body').append($lock);
            setSize();
            $lock.fadeIn(300);

            $(window).bind('resize', function () {
                setSize();
            });
        }

        //设置大小
        function setSize() {
            $lock.css({
                'opacity': options.opacity,
                'width': getWindowWidth(),
                'height': getPageHeight()
            });
        }

        //隐藏并销毁锁屏
        exports.hide = function () {
            $lock.fadeOut(JDialog.transitionTime, function () {
                $lock.remove();
            });
        }

        //获取元素 dom id
        exports.getId = function () {
            return options.id;
        }

        create();

        return exports;
    }

    /*************************************************************************
     * 提示框 msg 组件
     * ***********************************************************************
     * @param options
     * @private
     */
    var __message__ = function (options) {

        var o = {}; //导出的接口对象
        var $tipBox, $icon, $content, $end, $loading;

        //创建元素
        function create() {
            if ($tipBox != null) return;

            $tipBox = $('<div class="jtip_box animated" id="' + options.id + '"></div>');
            $icon = $('<div class="jtip_left_icon"><!--提示图标--></div>');
            $content = $('<div class="jtip_content"></div>');
            $end = $('<div class="jtip_right"><!--右边圆角--></div>');

            $tipBox.css('animation-name', options.effect); //设置特效

            $tipBox.append($icon);
            $tipBox.append($content);
            $tipBox.append($end);
            $('body').append($tipBox);

            $content.html(options.content);

            //确认消息类型
            switch (options.type) {
                case 'ok':
                    $icon.addClass('jtip_ok');
                    break;
                case 'error':
                    $icon.addClass('jtip_error');
                    break;
                case 'loading':
                    $loading = $('<em class="jtip_loading_icon"></em>');
                    var $loadBox = $('<div id="jtip_loading" class="jtip_load_img"></div>');
                    $loadBox.append($loading);
                    $icon.addClass('jtip_loading');
                    $loadBox.insertAfter($icon);
                    break;
                default:
                    $icon.addClass('jtip_warn');
                    break;

            }

            $tipBox.show();

        }

        //居中显示
        function center() {

            var width = options.width;
            if (!width) {
                width = $icon.width() + $content.width() + 21 + $end.width();
                if ($loading != null) {
                    width += $loading.width();
                }
            }
            $tipBox.setPosition(width, $tipBox.height(), options.offset, options.container);

        }

        //删除提示框
        o.hide = function () {
            $tipBox.css("animation-name", "zoomOut");
            setTimeout(function () {
                $tipBox.remove();
                if (typeof options.callback == 'function') {
                    options.callback(o);
                }
            }, JDialog.transitionTime);
            //隐藏锁屏工具
            if (o.locker != null) {
                o.locker.hide();
            }
        }

        //获取 message 对象 dom id
        o.getId = function () {
            return options.id;
        }

        //绑定窗口事件
        $(window).bind('resize', function () {
            center();
        });

        //锁屏提示
        if (options.lock) {
            o.locker = JDialog.lock();
        }

        create();
        center();

        return o;
    };

    /*************************************************************************
     * 弹出窗口组件
     * ***********************************************************************
     * @param __options
     * @returns {{}}
     * @private
     */
    var __JWindow__ = function (options) {

        var o = {};
        o.winBox = null; //窗体

        /* create elements */
        o.create = function () {

            o.winBox = $('<div id="' + options.id + '" class="jdialog_win_box jdialog_win_' + options.skin + ' animated"></div>');
            o.winBox.addClass("box-shadow"); //添加阴影和圆角

            //line 1, create title of window
            var title_box = $('<div class="jdialog_win_title_box"></div>');
            var win_title = $('<div class="jdialog_win_title">' + options.title + '</div>');

            var win_button = $('<div class="jdialog_win_button"></div>');
            var max_button = $('<a class="jdialog_win_max_button" href="javascript:void(0)" title="最大化"></a>');
            var close_button = $('<a class="jdialog_win_close_button" href="javascript:void(0)" title="关闭"></a>');

            //bind events for title button
            max_button.on("click", function (e) { o.resizeToMax(); e.stopPropagation(); }); //最大化窗口
            close_button.on("click", function () { o.close(); }); //关闭窗口

            if (options.maxEnable) {
                win_button.append(max_button);
            }
            if (options.closeBtn) {
                win_button.append(close_button);
            }
            title_box.append(win_title);
            title_box.append(win_button);
            if (options.hasTitle) {
                o.winBox.append(title_box);
            }

            //line 2, creat icon and content container
            var win_content = $('<div class="jdialog_win_CBOX"></div>');
            //add icon
            var icon = $('<span class="jdialog_win_icon"></span>');
            var content = $('<div class="jdialog_win_content"></div>');
            content.append(options.content);
            if (options.icon != "none") {
                win_content.append(icon);
            }
            win_content.append(content);
            o.winBox.append(win_content);

            //line 3, create button
            var btnBox = $('<div class="jdialog_win_button_container"></div>');
            var btn = $('<div class="jdialog_win_buttonInner"></div>');

            //create button
            if (options.button) {
                var JButton = function (name, callback, skin) {
                    skin = skin || 'primary';
                    var btn = $('<a href="javascript:void(0);" class="btn btn-' + skin + '">' + name + '</a>');
                    btn.click(function () {
                        callback(o);
                    });
                    return btn;
                }
                //put all buttons to button box
                for (var name in options.button) {
                    var __button;
                    if (isArray(options.button[name])) {
                        __button = new JButton(name, options.button[name][1], options.button[name][0])
                    } else {
                        __button = new JButton(name, options.button[name])
                    }
                    btn.append(__button);
                }
                btnBox.append(btn);
                o.winBox.append(btnBox);
            }

            o.winBox.data('me', {
                title: win_title,
                content: win_content,
                max_btn: max_button
            });

            //绑定拖动事件
            if (options.dragable) {
                title_box.mousedown(function (e) {
                    var offsetLeft = e.pageX - o.winBox.position().left;
                    var offsetTop = e.pageY - o.winBox.position().top;
                    $(document).mousemove(function (e) {
                        //清除拖动鼠标的时候选择文本
                        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
                        o.winBox.css({
                            'top': e.pageY - offsetTop + 'px',
                            'left': e.pageX - offsetLeft + 'px'
                        });
                    });
                }).mouseup(function () {
                    $(document).unbind('mousemove');
                });
            }

            $('body').append(o.winBox);
            //set the icon
            o.setIcon(options.icon);
        }

        /* set the icon of window */
        o.setIcon = function (itype) {

            if (!itype || itype == "none") return;
            o.winBox.find('.jdialog_win_icon').addClass("jdialog_win_icon_" + itype);
        }

        /* set position of window */
        o.setPosition = function () {

            //var left,top;
            var height = options.height > 0 ? options.height : o.winBox.height();
            if (options.height == 0 && options.button != undefined) {
                height += 60;   //加上按钮层的高度
            }
            var h = o.winBox.find(".jdialog_win_CBOX").height();
            if (h > 0) {
                height = o.winBox.find(".jdialog_win_title_box").outerHeight()
                    + o.winBox.find(".jdialog_win_CBOX").outerHeight()
                    + o.winBox.find(".jdialog_win_button_container").outerHeight();
            }
            options.height = height;

            o.winBox.setPosition(options.width, options.height, options.offset, options.container);

            //调整内容宽度
            if (options.icon == "none") {
                o.winBox.find(".jdialog_win_content").css({ width: "100%" });
            } else {
                o.winBox.find(".jdialog_win_content").css({ width: (o.winBox.width() - o.winBox.find(".jdialog_win_icon").width() - 50) + "px" });
            }

            // if (options.button) {
            // 	o.winBox.find(".jdialog_win_CBOX").height(options.height-115);
            // }
        }

        /* show the window */
        o.show = function () {
            if (o.winBox == null) return;

            if (options.lock) {
                o.locker = JDialog.lock();
            }
            o.winBox.css('animation-name', options.effect).show(); //设置出场特效
            o.winBox.css('display', 'flex');
            o.winBox.css('flex-direction', 'column');
        }

        /* clear the window's content */
        o.clear = function () {
            if (o.winBox == null) return;
            o.winBox.find('.jdialog_win_CBOX').empty();
            o.setPosition();
        }

        /* make window to max size */
        o.resizeToMax = function () {
            var data = o.winBox.data('smax');
            var max_btn = o.winBox.data('me').max_btn;
            if (data == undefined) {
                //记录最大化之前窗口的状态
                o.winBox.data('smax', {
                    top: o.winBox.position().top,
                    left: o.winBox.position().left,
                    width: o.winBox.width() + 2 * parseInt(o.winBox.css("border-width")),
                    height: o.winBox.height() + 2 * parseInt(o.winBox.css("border-width"))
                });

                //隐藏 body 滚动条
                $('body').css("overflow", "hidden");

                o.winBox.animate({
                    'top': getScrollTop(),
                    'left': 0,
                    'width': getWindowWidth(),
                    'height': getWindowHeight(),
                    'opacity': 1
                }, 'fast');
                //设置还原按钮属性
                max_btn.attr({
                    'class': 'jdialog_win_reduce_button',
                    'title': '还原'
                });

            } else {	//还原窗口

                o.winBox.animate({
                    'top': data.top,
                    'left': data.left,
                    'width': data.width,
                    'height': data.height,
                    'opacity': 1
                }, 'fast');
                //移除当前窗口状态
                o.winBox.removeData('smax');
                //设置最大化按钮属性
                max_btn.attr({
                    'class': 'jdialog_win_max_button',
                    'title': '最大化'
                });

                //恢复滚动条
                $('body').css("overflow", "auto");
            }

        }

        /* lock the window panel */
        o.lock = function () {
            var lock = $('<div class="jwindow-lock"></div>');
            var titleHeight = o.winBox.find('.jdialog_win_title_box').height();
            lock.css({
                top: titleHeight,
                left: 0,
                width: o.winBox.width() + 'px',
                height: o.winBox.height() - titleHeight + 'px',
            });
            o.winBox.append(lock);
        }

        /* unlock the window panel */
        o.unlock = function () {
            o.winBox.find(".jwindow-lock").remove();
        }

        //关闭窗体
        o.close = function () {
            o.winBox.css("animation-name", "zoomOut"); //退出动画
            setTimeout(function () {
                o.winBox.remove();
            }, JDialog.transitionTime);
            if (o.locker) {
                o.locker.hide();
            }
        }

        o.getId = function () {
            return options.id;
        }

        //绑定调整窗口大小事件
        $(window).bind('resize', function () {
            o.setPosition();
        });

        //初始化窗口
        o.create();
        o.setPosition();
        o.show();
        o.setPosition();
        o.options = options;
        return o;
    };

    var __loader__ = function (options) {
        var o = {};

        if (options.lock) {
            o.locker = JDialog.lock({
                'bgcolor': '#ffffff',
                'opacity': 0.3
            });
        }
        var $icon = $('<i id="' + options.id + '" class="loader-icon"></i>');
        $('body').append($icon);

        $icon.setPosition($icon.width(), $icon.height(), options.offset, options.container).addClass("loader-icon-" + options.skin);

        /**
         * 隐藏加载器
         */
        o.hide = function () {
            $icon.remove();
            if (o.locker) {
                o.locker.hide();
            }
            if (typeof options.callback == 'function') {
                options.callback();
            }
        }
        return o;
    }

    // 加载 css 文件
    var js = document.scripts, script = js[js.length - 1], jsPath = script.src;
    var cssPath = jsPath.substring(0, jsPath.lastIndexOf("/") + 1) + "css/JDialog.css"
    $("head:eq(0)").append('<link href="' + cssPath + '" rel="stylesheet" type="text/css" />');

    /**
     * 导出 JDialog 对象
     * @type {{transitionTime: number, lock: Window.JDialog.lock, msg: Window.JDialog.msg, open: Window.JDialog.open}}
     */
    window.JDialog = {

        margin: 10, //边距

        transitionTime: 300,  //动画过渡时间

        //锁屏
        lock: function (opts) {
            opts = opts || {};
            //默认参数
            var options = $.extend({
                id: generateElementId("lock"),
                timer: 0, //多长时间隐藏
                bgcolor: '#000000',
                opacity: 0.3,  //透明度
            }, opts);
            var o = new __locker__(options);
            if (options.timer > 0) {
                setTimeout(function () {
                    o.hide();
                }, options.timer);
            }
            return o;
        },

        //消息框
        msg: function (opts) {
            if (typeof opts == 'string') { //字符串类型的快捷调用
                opts = { content: opts }
            }
            opts = opts || {};
            //默认参数
            var options = $.extend({
                id: generateElementId("tip"),
                effect: 'zoomIn', //特效，可选[zoomIn, zoomInDown, bounceInDown, shake, flip, slideInDown]
                type: 'warn', //消息类别
                content: 'Hello, World.', //消息内容
                offset: 'cc',   //位置: 中部居中，可选参数：['cc', 'lt', 'tc', 'rt', 'lc', 'rc', 'lb', 'bc', 'rb']
                lock: false, //是否锁屏
                callback: function () { },
                timer: 2000 //显示时间
            }, opts);
            var msg = new __message__(options);
            if (options.timer > 0) {
                setTimeout(function () {
                    msg.hide();
                }, options.timer);
            }
            return msg;
        },

        //对话框
        open: function (opts) {

            if (typeof opts == 'string') { //字符串类型的快捷调用
                opts = { content: opts }
            }
            opts = opts || {};
            var options = $.extend({
                id: generateElementId("window"),
                title: 'This is the title',
                content: 'Hello, this is the content.',
                width: 600,
                height: 0, //窗口的高度：如果为0则为自适应
                hasTitle: true,    //是否出需要标题栏
                lock: true,   //弹出窗口的时候是否锁屏
                skin: 'default', //皮肤
                offset: 'cc', //位置: 中部居中,可选参数：['cc', 'lt', 'tc', 'rt', 'lc', 'rc', 'lb', 'bc', 'rb']
                effect: 'zoomIn', //特效，可选[zoomIn, zoomInDown, bounceInDown, shake, flip, slideInDown]
                maxEnable: false, //是否允许窗口最大化
                closeBtn: true, //右上角是否显示关闭按钮
                dragable: true, // 是否允许拖动
                /**
                 * 需要显示的图标，如果为none则表示不显示(默认),可选项
                 * ['warn','ok','edit','bag','ask','minus','italic','unlock','smile','angry','down','remove','msg','mail']
                 */
                icon: 'none',
                maxWidth: 1920
            }, opts);

            //按照百分比设置
            if (options.width < 100) {
                options.width = options.width * getWindowWidth() / 100;
            }
            if (options.height < 100) {
                options.height = options.height * getWindowHeight() / 100;
            }

            return new __JWindow__(options);
        },

        /**
         * 弹出提示框
         * @param options
         * @returns {*}
         */
        alert: function (options) {
            options = options || {};
            if (!options.width) {
                options.width = 300;
            }
            if (options.icon) {
                options.content = '<div style="padding-top:13px; font-size:16px;">' + options.content + '</div>';
            }
            options.button = {
                "确定": function () {
                    o.close();
                    //回调函数
                    if (typeof options.callback == "function") {
                        options.callback(o);
                    }
                }
            };

            var o = this.open(options);
            return o;
        },

        //confirm API
        confirm: function (options) {
            if (options.icon) {
                options.content = '<div style="padding-top:13px; font-size:16px;">' + options.content + '</div>';
            }
            if (!options.width) {
                options.width = 300;
            }
            options.closeBtn = true;
            return this.open(options);
        },

        //加载器
        loader: function (opts) {
            //默认参数
            var options = $.extend({
                id: generateElementId("loader"),
                skin: 4,
                container: '', //容器
                offset: 'cc', //位置
                lock: true, //是否锁屏
                timer: 2000 //显示时间
            }, opts);
            var o = new __loader__(options);
            if (options.timer) {
                setTimeout(function () {
                    o.hide();
                }, options.timer);
            }
            return o;
        }
    }

})(jQuery);
