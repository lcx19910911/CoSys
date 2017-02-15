/* 代码整理：大头网 www.datouwang.com */
; (function ($, window, document, undefined) {
    var pluginName = "tipso",
      defaults = {
          speed: 400,
          background: 'rgb(247, 25, 25)',
          color: '#ffffff',
          position: 'right',
          width: "auto",
          delay: 200,
          offsetX: 0,
          offsetY: 0,
          content: null,
          ajaxContentUrl: null,
          useTitle: true,
          onBeforeShow: null,
          onShow: null,
          onHide: null,
          isMouseover: true
      };

    function Plugin(element, options) {
        this.element = $(element);
        this.settings = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;
        this._title = this.element.attr('title');
        this.mode = 'hide';
        this.init();
    }
    $.extend(Plugin.prototype, {
        init: function () {
            var obj = this,
              $e = this.element;
            $e.removeAttr('title');
            if (this.settings.isMouseover) {
                if (isTouchSupported()) {
                    $e.on('click' + '.' + pluginName, function (e) {
                        obj.mode == 'hide' ? obj.show() : obj.hide();
                        e.stopPropagation();
                    });
                    $(document).on('click', function () {
                        if (obj.mode == 'show') {
                            obj.hide();
                        }
                    });
                } else {
                    $e.on('mouseover' + '.' + pluginName, function () {
                        obj.show();
                    });
                    $e.on('mouseout' + '.' + pluginName, function () {
                        obj.hide();
                    });
                }
            }
        },
        tooltip: function () {
            if (!this.tipso_bubble) {
                this.tipso_bubble = $(
                  '<div class="tipso_bubble"><div class="tipso_content"></div><div class="tipso_arrow"></div></div>'
                );
            }
            return this.tipso_bubble;
        },
        show: function () {
            var tipso_bubble = this.tooltip(),
              obj = this,
              $win = $(window);          
            tipso_bubble.css({
                background: obj.settings.background,
                color: obj.settings.color,
                width: obj.settings.width
            }).hide();
            tipso_bubble.find('.tipso_content').html(obj.content());
            reposition(obj);
            $win.resize(function () {
                reposition(obj);
            });
           
            obj.timeout = window.setTimeout(function () {
                tipso_bubble.appendTo(obj.element.parent());
                if ($.isFunction(obj.settings.onBeforeShow)) {
                    obj.settings.onBeforeShow($(this));
                }
                tipso_bubble.stop(true, true).fadeIn(obj.settings
                .speed, function () {
                    obj.mode = 'show';
                    if ($.isFunction(obj.settings.onShow)) {
                        obj.settings.onShow($(this));
                    }
                });
            }, obj.settings.delay);
        },
        hide: function () {
            var obj = this,
              tipso_bubble = this.tooltip();
            window.clearTimeout(obj.timeout);
            obj.timeout = null;
            tipso_bubble.stop(true, true).fadeOut(obj.settings.speed,
              function () {
                  $(this).remove();
                  if ($.isFunction(obj.settings.onHide) && obj.mode == 'show') {
                      obj.settings.onHide($(this));
                  }
                  obj.mode = 'hide';
              });
        },
        destroy: function () {
            var $e = this.element;
            $e.data("plugin_tipso").tipso_bubble.remove();
            $e.off('.' + pluginName);
            $e.removeData(pluginName);
            $e.attr('title', this._title);            
        },
        content: function () {
            var content,
              $e = this.element,
              obj = this,
              title = this._title;
            if (obj.settings.ajaxContentUrl) {
                content = $.ajax({
                    type: "GET",
                    url: obj.settings.ajaxContentUrl,
                    async: false
                }).responseText;
            } else if (obj.settings.content) {
                content = obj.settings.content;
            } else {
                if (obj.settings.useTitle === true) {
                    content = title;
                } else {
                    content = $e.data('tipso');
                }
            }
            return content;
        },
        update: function (key, value) {
            var obj = this;
            if (value) {
                obj.settings[key] = value;
            } else {
                return obj.settings[key];
            }
        }
    });

    function isTouchSupported() {
        var msTouchEnabled = window.navigator.msMaxTouchPoints;
        var generalTouchEnabled = "ontouchstart" in document.createElement(
          "div");
        if (msTouchEnabled || generalTouchEnabled) {
            return true;
        }
        return false;
    }

    function realHeight(obj) {
        var clone = obj.clone();
        clone.css("visibility", "hidden");
        $('body').append(clone);
        var height = clone.outerHeight();
        clone.remove();
        return height;
    }

    function reposition(thisthat) {
        var tipso_bubble = thisthat.tooltip(),
          $e = thisthat.element,
          obj = thisthat,
          $win = $(window),
          arrow = 10,
          pos_top, pos_left, diff;
        switch (obj.settings.position) {
            case 'top':
                pos_left = $e.position().left + ($e.outerWidth() / 2) - (tipso_bubble
                  .outerWidth() / 2);
                pos_top = $e.position().top - realHeight(tipso_bubble) - arrow;
                tipso_bubble.find('.tipso_arrow').css({
                    marginLeft: -8
                });

                tipso_bubble.find('.tipso_arrow').css({
                    'border-top-color': obj.settings.background,
                    'border-bottom-color': 'transparent',
                    'border-right-color': 'transparent',
                    'border-left-color': 'transparent',
                });
                tipso_bubble.removeClass('top bottom left right');
                tipso_bubble.addClass(obj.settings.position);

                break;
            case 'bottom':
                pos_left = $e.position().left + ($e.outerWidth() / 2) - (tipso_bubble
                  .outerWidth() / 2);
                pos_top = $e.position().top + $e.outerHeight() + arrow;
                tipso_bubble.find('.tipso_arrow').css({
                    marginLeft: -8
                });

                tipso_bubble.find('.tipso_arrow').css({
                    'border-bottom-color': obj.settings.background,
                    'border-top-color': 'transparent',
                    'border-right-color': 'transparent',
                    'border-left-color': 'transparent',
                });
                tipso_bubble.removeClass('top bottom left right');
                tipso_bubble.addClass(obj.settings.position);

                break;
            case 'left':
                pos_left = $e.position().left - tipso_bubble.outerWidth() - arrow;
                pos_top = $e.position().top + ($e.outerHeight() / 2) - (realHeight(
                  tipso_bubble) / 2);
                tipso_bubble.find('.tipso_arrow').css({
                    marginTop: -8,
                    marginLeft: ''
                });

                tipso_bubble.find('.tipso_arrow').css({
                    'border-left-color': obj.settings.background,
                    'border-right-color': 'transparent',
                    'border-top-color': 'transparent',
                    'border-bottom-color': 'transparent'
                });
                tipso_bubble.removeClass('top bottom left right');
                tipso_bubble.addClass(obj.settings.position);

                break;
            case 'right':
                pos_left = $e.position().left + $e.outerWidth() + arrow;
                pos_top = $e.position().top + ($e.outerHeight() / 2) - (realHeight(
                  tipso_bubble) / 2);
                tipso_bubble.find('.tipso_arrow').css({
                    marginTop: -8,
                    marginLeft: ''
                });

                tipso_bubble.find('.tipso_arrow').css({
                    'border-right-color': obj.settings.background,
                    'border-left-color': 'transparent',
                    'border-top-color': 'transparent',
                    'border-bottom-color': 'transparent'
                });
                tipso_bubble.removeClass('top bottom left right');
                tipso_bubble.addClass(obj.settings.position);

                break;
        }


        tipso_bubble.css({
            left: pos_left + obj.settings.offsetX,
            top: pos_top + obj.settings.offsetY
        });
    }
    $[pluginName] = $.fn[pluginName] = function (options) {
        var args = arguments;
        if (options === undefined || typeof options === 'object') {
            if (!(this instanceof $)) {
                $.extend(defaults, options);
            }
            return this.each(function () {
                if (!$.data(this, 'plugin_' + pluginName)) {
                    $.data(this, 'plugin_' + pluginName, new Plugin(this, options));
                }
            });
        } else if (typeof options === 'string' && options[0] !== '_' && options !==
          'init') {
            var returns;
            this.each(function () {
                var instance = $.data(this, 'plugin_' + pluginName);
                if (!instance) {
                    instance = $.data(this, 'plugin_' + pluginName, new Plugin(
                      this, options));
                }
                if (instance instanceof Plugin && typeof instance[options] ===
                  'function') {
                    returns = instance[options].apply(instance, Array.prototype.slice
                      .call(args, 1));
                }
                if (options === 'destroy') {
                    $.data(this, 'plugin_' + pluginName, null);
                }
            });
            return returns !== undefined ? returns : this;
        }
    };
})(jQuery, window, document);