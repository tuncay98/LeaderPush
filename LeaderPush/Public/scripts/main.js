/*
*
*  Push Notifications codelab
*  Copyright 2015 Google Inc. All rights reserved.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      https://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License
*
*/

/* eslint-env browser, es6 */

'use strict';

/*if ('serviceWorker' in navigator && 'PushManager' in window) {
  console.log('Service Worker and Push is supported');

  navigator.serviceWorker.register('sw.js')
  .then(function(swReg) {
    console.log('Service Worker is registered', swReg);

    swRegistration = swReg;
  })
  .catch(function(error) {
    console.error('Service Worker Error', error);
  });
} else {
  console.warn('Push messaging is not supported');
  pushButton.textContent = 'Push Not Supported';
}
*/


/*function initializeUI() {
  // Set the initial subscription value
  swRegistration.pushManager.getSubscription()
  .then(function(subscription) {
    isSubscribed = !(subscription === null);

    if (isSubscribed) {
      console.log('User IS subscribed.');
    } else {
      console.log('User is NOT subscribed.');
    }

    updateBtn();
  });
}*/

var Inc;

function isIncognito(callback) {
    var fs = window.RequestFileSystem || window.webkitRequestFileSystem;

    if (!fs) {
        callback(false);
    } else {
        fs(window.TEMPORARY,
            100,
            callback.bind(undefined, false),
            callback.bind(undefined, true)
        );
    }
}

isIncognito(function (itIs) {
    if (itIs) {
        console.log("Please, do not use the incognito mode for a push notification")
    } else {

        const applicationServerPublicKey = 'BAS21PmINNueC0awLLy-VE6FMManBBJMZT6NUAwb3Ou0gsAuuMrYwiJGnW4f__ew1yIh1VnXVwCNTpFDig6i7eA';


        function initializeUI() {
            //  pushButton.addEventListener('click', function () {
            // pushButton.disabled = true;
            if (isSubscribed) {
                // TODO: Unsubscribe user
            } else {
                subscribeUser();
            }
            //});

            // Set the initial subscription value
            swRegistration.pushManager.getSubscription()
                .then(function (subscription) {
                    isSubscribed = !(subscription === null);

                    updateSubscriptionOnServer(subscription);

                    if (isSubscribed) {
                        console.log('User IS subscribed.');
                    } else {
                        console.log('User is NOT subscribed.');
                    }

                    updateBtn();
                });
        }



        function updateBtn() {

            if (Notification.permission === 'denied') {
                // pushButton.textContent = 'Push Messaging Blocked.';
                //  pushButton.disabled = true;
                updateSubscriptionOnServer(null);
                return;
            }

            if (isSubscribed) {
                //   pushButton.textContent = 'Disable Push Messaging';
            } else {
                // pushButton.textContent = 'Enable Push Messaging';
            }

            // pushButton.disabled = false;
        }
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        }

        var foo = getParameterByName('url');
        var dom = document.domain;

        var link = "https://" + dom + "/apps/leaderpush/public/sw.js";

        navigator.serviceWorker.register(link)
            .then(function (swReg) {
                console.log('Service Worker is registered', swReg);

                swRegistration = swReg;
                initializeUI();
            })


        function subscribeUser() {
            const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
            swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerKey
            })
                .then(function (subscription) {
                    console.log('User is subscribed.');

                    updateSubscriptionOnServer(subscription);

                    isSubscribed = true;

                    updateBtn();
                })
                .catch(function (err) {
                    console.log('Failed to subscribe the user: ', err);
                    updateBtn();
                });
        }

        function createCORSRequest(method, url) {
            var xhr = new XMLHttpRequest();
            if ("withCredentials" in xhr) {
                // XHR has 'withCredentials' property only if it supports CORS
                xhr.open(method, url, true);
            } else if (typeof XDomainRequest != "undefined") { // if IE use XDR
                xhr = new XDomainRequest();
                xhr.open(method, url);
            } else {
                xhr = null;
            }
            return xhr;
        }

        function updateSubscriptionOnServer(subscription) {
            // TODO: Send subscription to application server

            // const subscriptionJson = document.querySelector('.js-subscription-json');
            // const subscriptionDetails =
            //    document.querySelector('.js-subscription-details');

            if (subscription) {
                // subscriptionJson.textContent = JSON.stringify(subscription);

                var t = JSON.stringify(subscription);
                var obj = JSON.parse(t);
                var endpoint = obj.endpoint;
                var p256dh = obj.keys.p256dh;
                var auth = obj.keys.auth;



                var domain = document.domain;

                var request = createCORSRequest("get", '//www.leaderpush.com/Send/Getjson?endpoint=' + endpoint + '&p256dh=' + p256dh + '&auth=' + auth + '&domain=' + domain);
                if (request) {
                    // Define a callback function
                    request.onload = function () { };
                    // Send request
                    request.send();
                }




                //subscriptionDetails.classList.remove('is-invisible');
            } else {
                // subscriptionDetails.classList.add('is-invisible');
            }
        }



        //const pushButton = document.querySelector('.js-push-btn');

        let isSubscribed = false;
        let swRegistration = null;




        function urlB64ToUint8Array(base64String) {
            const padding = '='.repeat((4 - base64String.length % 4) % 4);
            const base64 = (base64String + padding)
                .replace(/\-/g, '+')
                .replace(/_/g, '/');

            const rawData = window.atob(base64);
            const outputArray = new Uint8Array(rawData.length);

            for (let i = 0; i < rawData.length; ++i) {
                outputArray[i] = rawData.charCodeAt(i);
            }
            return outputArray;
        }





    }
});












