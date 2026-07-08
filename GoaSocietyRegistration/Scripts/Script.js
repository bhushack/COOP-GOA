jQuery(function ($) {

    'use strict';

    /* ---------------------------------------------- /*
     * Preloader
    /* ---------------------------------------------- */

    $(window).ready(function () {
        $('#status').fadeOut();
        $('#preloader').delay(200).fadeOut('slow');
    });

    /* ---------------------------------------------- /*
     * Bootstrap Tooltip
    /* ---------------------------------------------- */

    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    // -------------------------------------------------------------
    // Sticky Menu
    // -------------------------------------------------------------

    (function () {
        var nav = $('.navbar');
        var scrolled = false;

        $(window).scroll(function () {

            if (110 < $(window).scrollTop() && !scrolled) {
                nav.addClass('sticky animated fadeInDown').animate({ 'margin-top': '0px' });

                scrolled = true;
            }

            if (110 > $(window).scrollTop() && scrolled) {
                nav.removeClass('sticky animated fadeInDown').css('margin-top', '0px');

                scrolled = false;
            }
        });

    }());


    // -------------------------------------------------------------
    // Portfolio Carousel
    // -------------------------------------------------------------
    (function () {

        $('.fleet-carousel').owlCarousel({
            loop: true,
            margin: 0,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 2
                },
                1000: {
                    items: 4
                }
            }
        })


        // Navigation
        var owl = $('.fleet-carousel');
        owl.owlCarousel();

        // Go to the next item
        $('.fleet-carousel-navigation .next').on('click', function () {
            owl.trigger('next.owl.carousel');
        })

        // Go to the previous item
        $('.fleet-carousel-navigation .prev').on('click', function () {
            owl.trigger('prev.owl.carousel', [300]);
        });


    }());





    // -------------------------------------------------------------
    // Sticky Elements
    // -------------------------------------------------------------

    $(function () {

        $(".sticky-div").stick_in_parent({

            container: $(".sticky-container"),
            offset_top: 90
        });


    });


    // -------------------------------------------------------------
    // Menu Search Button
    // -------------------------------------------------------------

    $(function () {
        $('a[href="#search"]').on('click', function (event) {
            event.preventDefault();
            $('#search').addClass('open');
            $('#search > form > input[type="search"]').focus();
        });

        $('#search, #search button.close').on('click keyup', function (event) {
            if (event.target == this || event.target.className == 'close' || event.keyCode == 27) {
                $(this).removeClass('open');
            }
        });

    });


    // -------------------------------------------------------------
    // Magnific  Popup
    // -------------------------------------------------------------

    (function () {
        $('.img-link').magnificPopup({

            gallery: {
                enabled: true
            },
            removalDelay: 300, // Delay in milliseconds before popup is removed
            mainClass: 'mfp-with-zoom', // this class is for CSS animation below
            type: 'image'

        });
    }());


    // -------------------------------------------------------------
    // Language Select JS
    // -------------------------------------------------------------

    (function () {
        [].slice.call(document.querySelectorAll('select.lang-select')).forEach(function (el) {
            new SelectFx(el);
        });
    })();


    // -------------------------------------------------------------
    // OffCanvas
    // -------------------------------------------------------------

    (function () {
        $('button.navbar-toggle').HippoOffCanvasMenu({

            documentWrapper: '#st-container',
            contentWrapper: '.st-content',
            position: 'hippo-offcanvas-left',    // class name
            // opener         : 'st-menu-open',            // class name
            effect: 'slide-in-on-top',             // class name
            closeButton: '#off-canvas-close-btn',
            menuWrapper: '.offcanvas-menu',                 // class name below-pusher
            documentPusher: '.st-pusher'

        });
        var ico = $('<i class="fa fa-caret-right"></i>');
        $('#offcanvasMenu li:has(ul) > a').append(ico);

        $('#offcanvasMenu > li:has(ul)').on('click', function () {
            $(this).toggleClass('open');
        });
    }());


    // -------------------------------------------------------------
    // Detect IE version
    // -------------------------------------------------------------
    (function () {
        function getIEVersion() {
            var match = navigator.userAgent.match(/(?:MSIE |Trident\/.*; rv:)(\d+)/);
            return match ? parseInt(match[1]) : false;
        }


        if (getIEVersion()) {
            $('html').addClass('ie' + getIEVersion());
        }


        if ($('html').hasClass('ie9') || $('html').hasClass('ie10')) {

            $('.submenu-wrapper').each(function () {

                $(this).addClass('no-pointer-events');

            });

        }

    }());

    // ------------------------------------------------------------------
    // jQuery for back to Top
    // ------------------------------------------------------------------

    (function () {

        $('body').append('<div id="toTop"><i class="fa fa-angle-double-up"></i></div>');

        $(window).scroll(function () {
            if ($(this).scrollTop() != 0) {
                $('#toTop').fadeIn();
            } else {
                $('#toTop').fadeOut();
            }
        });

        $('#toTop').on('click', function () {
            $("html, body").animate({ scrollTop: 0 }, 600);
            return false;
        });

    }());

    // -----------------------------------------------------------------
    //STELLAR FOR BACKGROUND SCROLLING
    // ------------------------------------------------------------------

    $(window).load(function () {

        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {

        } else {
            $.stellar({
                horizontalScrolling: false,
                responsive: true
            });
        }

    });

    // ----------------------------------------------------------------
    //  Dropdown menu
    // ----------------------------------------------------------------

    (function () {


        function getIEVersion() {
            var match = navigator.userAgent.match(/(?:MSIE |Trident\/.*; rv:)(\d+)/);
            return match ? parseInt(match[1]) : false;
        }


        if (getIEVersion()) {
            $('html').addClass('ie' + getIEVersion());
        }


        if ($('html').hasClass('ie9') || $('html').hasClass('ie10')) {

            $('.submenu-wrapper').each(function () {

                $(this).addClass('no-pointer-events');

            });

        }


        var timer;

        $('li.dropdown').on('mouseenter', function (event) {


            event.stopImmediatePropagation();
            event.stopPropagation();

            $(this).removeClass('open menu-animating').addClass('menu-animating');
            var that = this;


            if (timer) {
                clearTimeout(timer);
                timer = null;
            }

            timer = setTimeout(function () {

                $(that).removeClass('menu-animating');
                $(that).addClass('open');

            }, 300);   // 300ms as animation end time


        });

        // on mouse leave

        $('li.dropdown').on('mouseleave', function (event) {

            var that = this;

            $(this).removeClass('open menu-animating').addClass('menu-animating');


            if (timer) {
                clearTimeout(timer);
                timer = null;
            }

            timer = setTimeout(function () {

                $(that).removeClass('menu-animating');
                $(that).removeClass('open');

            }, 300);  // 300ms as animation end time

        });

    }());



    // -----------------------------------------------------------------
    //CONTACT FORM
    // ------------------------------------------------------------------

    (function () {

        $('#contactForm').on('submit', function (e) {

            e.preventDefault();

            var $action = $(this).prop('action');
            var $data = $(this).serialize();
            var $this = $(this);

            $this.prevAll('.alert').remove();

            $.post($action, $data, function (data) {

                console.log(data);

                if (data.response == 'error') {

                    $this.before('<div class="alert alert-warning">' + data.message + '</div>');
                }

                if (data.response == 'success') {

                    $this.before('<div class="alert alert-success">' + data.message + '</div>');
                    $this.find('input, textarea').val('');
                }

            }, "json");

        });
    }());

    // Twitter
    (function () {
        var twitterConfig = {
            id: "567185781790228482", //put your Widget ID here
            domId: "twitterWidget",
            maxTweets: 3,
            enableLinks: true,
            showUser: false,
            showTime: true,
            showInteraction: false,
            customCallback: handleTweets
        };
        twitterFetcher.fetch(twitterConfig);

        function handleTweets(tweets) {
            var x = tweets.length;
            var n = 0;
            var html = "";
            while (n < x) {
                html += '<div class="item">' + tweets[n] +
                    "</div>";
                n++
            }
            $(".twitter-widget").html(html);
            $(".twitter_retweet_icon").html(
                '<i class="fa fa-retweet"></i>');
            $(".twitter_reply_icon").html(
                '<i class="fa fa-reply"></i>');
            $(".twitter_fav_icon").html(
                '<i class="fa fa-star"></i>');
            $(".twitter-widget").owlCarousel({
                items: 1,
                loop: true,
                autoplay: true
            });

        }
    }());

    // Google map
    if ($('#googleMap').length > 0) {

        //set your google maps parameters
        var $latitude = 48.869319, //If you unable to find latitude and longitude of your address. Please visit http://www.latlong.net/convert-address-to-lat-long.html you can easily generate.
            $longitude = 2.354261,
            $map_zoom = 16; /* ZOOM SETTING */

        //google map custom marker icon
        var $marker_url = 'img/map-marker.png';

        //we define here the style of the map
        var style = [{
            "stylers": [{
                "hue": "#65d3e3"
            }, {
                "saturation": -10
            }, {
                "gamma": 2.15
            }, {
                "lightness": 12
            }]
        }];

        //set google map options
        var map_options = {
            center: new google.maps.LatLng($latitude, $longitude),
            zoom: $map_zoom,
            panControl: true,
            zoomControl: true,
            mapTypeControl: false,
            streetViewControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            scrollwheel: false,
            styles: style,
        }


        //initialize the contact page map
        var mainMap = new google.maps.Map(document.getElementById('googleMap'), map_options);
        new google.maps.Marker({
            position: new google.maps.LatLng($latitude, $longitude),
            map: mainMap,
            visible: true,
            icon: $marker_url,
        });
    }
}); // JQuery end

// overlay and preloader
function CoverClick1() {
    el = this.document.getElementById("CoverDoubleClick");
    el.style.visibility = (el.style.visibility == "visible") ? "hidden" : "visible";
    el.style.display = (el.style.display == "none") ? "block" : "none";
    cov = this.document.getElementById("cover");
    cov.style.visibility = (cov.style.visibility == "visible") ? "hidden" : "visible";
    cov.style.display = (el.style.display == "none") ? "block" : "none";
}
function overlay(actionType, actionPanel) {
    var myHidden = this.document.getElementById('hdnAction_type');

    if (actionType == 'close') {
        myHidden.value = '';
        $("[name*='lblActionStatus']").html("");
    }
    else if (actionType == 'forward') {
        myHidden.value = 'login';
        $("[name*='lblActionStatus']").html("");
    }
    else if (actionType == 'status')
        myHidden.value = '';

    el = this.document.getElementById("overlay");
    el.style.visibility = (el.style.visibility == "visible") ? "hidden" : "visible";
    el.style.display = (el.style.display == "none") ? "block" : "none";

    cov = this.document.getElementById("cover");
    cov.style.visibility = (cov.style.visibility == "visible") ? "hidden" : "visible";
    cov.style.display = (el.style.display == "none") ? "block" : "none";

    if (actionPanel == 'pnlActionStatus') {
        this.document.getElementById("actiondiv").style.display = "none";
        this.document.getElementById("actionStatus").style.visibility = "visible";
        this.document.getElementById("actionStatus").style.display = "block";
    }
    else {
        this.document.getElementById("actiondiv").style.display = "block";
        this.document.getElementById("actionStatus").style.visibility = "hidden";
        this.document.getElementById("actionStatus").style.display = "none";
    }

    window.scrollTo(0, 0);
}
function overlay1(actionType, actionPanel) {
    var myHidden = this.document.getElementById('hdnAction_type');

    if (actionType == 'close') {
        myHidden.value = '';
        $("[name*='lblActionStatus1']").html("");
    }
    else if (actionType == 'forward') {
        myHidden.value = 'login';
        $("[name*='lblActionStatus1']").html("");
    }
    else if (actionType == 'status')
        myHidden.value = '';

    el = this.document.getElementById("overlay1");
    el.style.visibility = (el.style.visibility == "visible") ? "hidden" : "visible";
    el.style.display = (el.style.display == "none") ? "block" : "none";

    cov = this.document.getElementById("cover");
    cov.style.visibility = (cov.style.visibility == "visible") ? "hidden" : "visible";
    cov.style.display = (el.style.display == "none") ? "block" : "none";

    if (actionPanel == 'pnlActionStatus') {
        this.document.getElementById("actiondiv1").style.display = "none";
        this.document.getElementById("actionStatus1").style.visibility = "visible";
        this.document.getElementById("actionStatus1").style.display = "block";
    }
    else {
        this.document.getElementById("actiondiv1").style.display = "block";
        this.document.getElementById("actionStatus1").style.visibility = "hidden";
        this.document.getElementById("actionStatus1").style.display = "none";
    }

    window.scrollTo(0, 0);
}
function UncoverClick() {
    el = this.document.getElementById("CoverDoubleClick");
    el.style.visibility = "hidden";
    el.style.display = "none";

    cov = this.document.getElementById("cover");
    cov.style.visibility = "hidden";
    cov.style.display = "none";
}