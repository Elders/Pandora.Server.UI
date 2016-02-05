!function(t,i,e){"use strict";var s,o,n,l,r,h,c,a,p,d,u,g,v,f,S,m,T,y,b,w,$,x,H,C,O,E,A,Y,D;$={paneClass:"nano-pane",sliderClass:"nano-slider",contentClass:"nano-content",iOSNativeScrolling:!1,preventPageScrolling:!1,disableResize:!1,alwaysVisible:!1,flashDelay:1500,sliderMinHeight:20,sliderMaxHeight:null,documentContext:null,windowContext:null},m="scrollbar",S="scroll",a="mousedown",p="mousemove",u="mousewheel",d="mouseup",f="resize",r="drag",y="up",v="panedown",n="DOMMouseScroll",l="down",b="wheel",h="keydown",c="keyup",T="touchmove",s="Microsoft Internet Explorer"===i.navigator.appName&&/msie 7./i.test(i.navigator.appVersion)&&i.ActiveXObject,o=null,O=i.requestAnimationFrame,w=i.cancelAnimationFrame,A=e.createElement("div").style,D=function(){var t,i,e,s,o,n;for(s=["t","webkitT","MozT","msT","OT"],t=o=0,n=s.length;n>o;t=++o)if(e=s[t],i=s[t]+"ransform",i in A)return s[t].substr(0,s[t].length-1);return!1}(),Y=function(t){return D===!1?!1:""===D?t:D+t.charAt(0).toUpperCase()+t.substr(1)},E=Y("transform"),H=E!==!1,x=function(){var t,i,s;return t=e.createElement("div"),i=t.style,i.position="absolute",i.width="100px",i.height="100px",i.overflow=S,i.top="-9999px",e.body.appendChild(t),s=t.offsetWidth-t.clientWidth,e.body.removeChild(t),s},C=function(){var t,e,s;return e=i.navigator.userAgent,(t=/(?=.+Mac OS X)(?=.+Firefox)/.test(e))?(s=/Firefox\/\d{2}\./.exec(e),s&&(s=s[0].replace(/\D+/g,"")),t&&+s>23):!1},g=function(){function h(s,n){this.el=s,this.options=n,o||(o=x()),this.$el=t(this.el),this.doc=t(this.options.documentContext||e),this.win=t(this.options.windowContext||i),this.$content=this.$el.children("."+n.contentClass),this.$content.attr("tabindex",this.options.tabIndex||0),this.content=this.$content[0],this.previousPosition=0,this.options.iOSNativeScrolling&&null!=this.el.style.WebkitOverflowScrolling?this.nativeScrolling():this.generate(),this.createEvents(),this.addEvents(),this.reset()}return h.prototype.preventScrolling=function(t,i){if(this.isActive)if(t.type===n)(i===l&&t.originalEvent.detail>0||i===y&&t.originalEvent.detail<0)&&t.preventDefault();else if(t.type===u){if(!t.originalEvent||!t.originalEvent.wheelDelta)return;(i===l&&t.originalEvent.wheelDelta<0||i===y&&t.originalEvent.wheelDelta>0)&&t.preventDefault()}},h.prototype.nativeScrolling=function(){this.$content.css({WebkitOverflowScrolling:"touch"}),this.iOSNativeScrolling=!0,this.isActive=!0},h.prototype.updateScrollValues=function(){var t,i;t=this.content,this.maxScrollTop=t.scrollHeight-t.clientHeight,this.prevScrollTop=this.contentScrollTop||0,this.contentScrollTop=t.scrollTop,i=this.contentScrollTop>this.previousPosition?"down":this.contentScrollTop<this.previousPosition?"up":"same",this.previousPosition=this.contentScrollTop,"same"!==i&&this.$el.trigger("update",{position:this.contentScrollTop,maximum:this.maxScrollTop,direction:i}),this.iOSNativeScrolling||(this.maxSliderTop=this.paneHeight-this.sliderHeight,this.sliderTop=0===this.maxScrollTop?0:this.contentScrollTop*this.maxSliderTop/this.maxScrollTop)},h.prototype.setOnScrollStyles=function(){var t;H?(t={},t[E]="translate(0, "+this.sliderTop+"px)"):t={top:this.sliderTop},O?this.scrollRAF||(this.scrollRAF=O(function(i){return function(){i.scrollRAF=null,i.slider.css(t)}}(this))):this.slider.css(t)},h.prototype.createEvents=function(){this.events={down:function(t){return function(i){return t.isBeingDragged=!0,t.offsetY=i.pageY-t.slider.offset().top,t.pane.addClass("active"),t.doc.bind(p,t.events[r]).bind(d,t.events[y]),!1}}(this),drag:function(t){return function(i){return t.sliderY=i.pageY-t.$el.offset().top-t.offsetY,t.scroll(),t.contentScrollTop>=t.maxScrollTop&&t.prevScrollTop!==t.maxScrollTop?t.$el.trigger("scrollend"):0===t.contentScrollTop&&0!==t.prevScrollTop&&t.$el.trigger("scrolltop"),!1}}(this),up:function(t){return function(i){return t.isBeingDragged=!1,t.pane.removeClass("active"),t.doc.unbind(p,t.events[r]).unbind(d,t.events[y]),!1}}(this),resize:function(t){return function(i){t.reset()}}(this),panedown:function(t){return function(i){return t.sliderY=(i.offsetY||i.originalEvent.layerY)-.5*t.sliderHeight,t.scroll(),t.events.down(i),!1}}(this),scroll:function(t){return function(i){t.updateScrollValues(),t.isBeingDragged||(t.iOSNativeScrolling||(t.sliderY=t.sliderTop,t.setOnScrollStyles()),null!=i&&(t.contentScrollTop>=t.maxScrollTop?(t.options.preventPageScrolling&&t.preventScrolling(i,l),t.prevScrollTop!==t.maxScrollTop&&t.$el.trigger("scrollend")):0===t.contentScrollTop&&(t.options.preventPageScrolling&&t.preventScrolling(i,y),0!==t.prevScrollTop&&t.$el.trigger("scrolltop"))))}}(this),wheel:function(t){return function(i){var e;if(null!=i)return e=i.delta||i.wheelDelta||i.originalEvent&&i.originalEvent.wheelDelta||-i.detail||i.originalEvent&&-i.originalEvent.detail,e&&(t.sliderY+=-e/3),t.scroll(),!1}}(this)}},h.prototype.addEvents=function(){var t;this.removeEvents(),t=this.events,this.options.disableResize||this.win.bind(f,t[f]),this.iOSNativeScrolling||(this.slider.bind(a,t[l]),this.pane.bind(a,t[v]).bind(""+u+" "+n,t[b])),this.$content.bind(""+S+" "+u+" "+n+" "+T,t[S])},h.prototype.removeEvents=function(){var t;t=this.events,this.win.unbind(f,t[f]),this.iOSNativeScrolling||(this.slider.unbind(),this.pane.unbind()),this.$content.unbind(""+S+" "+u+" "+n+" "+T,t[S])},h.prototype.generate=function(){var t,e,s,n,l,r,h;return n=this.options,r=n.paneClass,h=n.sliderClass,t=n.contentClass,(l=this.$el.children("."+r)).length||l.children("."+h).length||this.$el.append('<div class="'+r+'"><div class="'+h+'" /></div>'),this.pane=this.$el.children("."+r),this.slider=this.pane.find("."+h),0===o&&C()?(s=i.getComputedStyle(this.content,null).getPropertyValue("padding-right").replace(/\D+/g,""),e={right:-14,paddingRight:+s+14}):o&&(e={right:-o},this.$el.addClass("has-scrollbar")),null!=e&&this.$content.css(e),this},h.prototype.restore=function(){this.stopped=!1,this.iOSNativeScrolling||this.pane.show(),this.addEvents()},h.prototype.reset=function(){var t,i,e,n,l,r,h,c,a,p,d,u;return this.iOSNativeScrolling?void(this.contentHeight=this.content.scrollHeight):(this.$el.find("."+this.options.paneClass).length||this.generate().stop(),this.stopped&&this.restore(),t=this.content,n=t.style,l=n.overflowY,s&&this.$content.css({height:this.$content.height()}),i=t.scrollHeight+o,p=parseInt(this.$el.css("max-height"),10),p>0&&(this.$el.height(""),this.$el.height(t.scrollHeight>p?p:t.scrollHeight)),h=this.pane.outerHeight(!1),a=parseInt(this.pane.css("top"),10),r=parseInt(this.pane.css("bottom"),10),c=h+a+r,u=Math.round(c/i*c),u<this.options.sliderMinHeight?u=this.options.sliderMinHeight:null!=this.options.sliderMaxHeight&&u>this.options.sliderMaxHeight&&(u=this.options.sliderMaxHeight),l===S&&n.overflowX!==S&&(u+=o),this.maxSliderTop=c-u,this.contentHeight=i,this.paneHeight=h,this.paneOuterHeight=c,this.sliderHeight=u,this.slider.height(u),this.events.scroll(),this.pane.show(),this.isActive=!0,t.scrollHeight===t.clientHeight||this.pane.outerHeight(!0)>=t.scrollHeight&&l!==S?(this.pane.hide(),this.isActive=!1):this.el.clientHeight===t.scrollHeight&&l===S?this.slider.hide():this.slider.show(),this.pane.css({opacity:this.options.alwaysVisible?1:"",visibility:this.options.alwaysVisible?"visible":""}),e=this.$content.css("position"),("static"===e||"relative"===e)&&(d=parseInt(this.$content.css("right"),10),d&&this.$content.css({right:"",marginRight:d})),this)},h.prototype.scroll=function(){return this.isActive?(this.sliderY=Math.max(0,this.sliderY),this.sliderY=Math.min(this.maxSliderTop,this.sliderY),this.$content.scrollTop((this.paneHeight-this.contentHeight+o)*this.sliderY/this.maxSliderTop*-1),this.iOSNativeScrolling||(this.updateScrollValues(),this.setOnScrollStyles()),this):void 0},h.prototype.scrollBottom=function(t){return this.isActive?(this.$content.scrollTop(this.contentHeight-this.$content.height()-t).trigger(u),this.stop().restore(),this):void 0},h.prototype.scrollTop=function(t){return this.isActive?(this.$content.scrollTop(+t).trigger(u),this.stop().restore(),this):void 0},h.prototype.scrollTo=function(t){return this.isActive?(this.scrollTop(this.$el.find(t).get(0).offsetTop),this):void 0},h.prototype.stop=function(){return w&&this.scrollRAF&&(w(this.scrollRAF),this.scrollRAF=null),this.stopped=!0,this.removeEvents(),this.iOSNativeScrolling||this.pane.hide(),this},h.prototype.destroy=function(){return this.stopped||this.stop(),!this.iOSNativeScrolling&&this.pane.length&&this.pane.remove(),s&&this.$content.height(""),this.$content.removeAttr("tabindex"),this.$el.hasClass("has-scrollbar")&&(this.$el.removeClass("has-scrollbar"),this.$content.css({right:""})),this},h.prototype.flash=function(){return!this.iOSNativeScrolling&&this.isActive?(this.reset(),this.pane.addClass("flashed"),setTimeout(function(t){return function(){t.pane.removeClass("flashed")}}(this),this.options.flashDelay),this):void 0},h}(),t.fn.nanoScroller=function(i){return this.each(function(){var e,s;if((s=this.nanoscroller)||(e=t.extend({},$,i),this.nanoscroller=s=new g(this,e)),i&&"object"==typeof i){if(t.extend(s.options,i),null!=i.scrollBottom)return s.scrollBottom(i.scrollBottom);if(null!=i.scrollTop)return s.scrollTop(i.scrollTop);if(i.scrollTo)return s.scrollTo(i.scrollTo);if("bottom"===i.scroll)return s.scrollBottom(0);if("top"===i.scroll)return s.scrollTop(0);if(i.scroll&&i.scroll instanceof t)return s.scrollTo(i.scroll);if(i.stop)return s.stop();if(i.destroy)return s.destroy();if(i.flash)return s.flash()}return s.reset()})},t.fn.nanoScroller.Constructor=g}(jQuery,window,document);