mergeInto(LibraryManager.library, {
    YandexGames_Bind: function (gameObjectNamePtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        window.__ygUnityObject = gameObjectName;

        var send = function (method, arg) {
            try {
                if (typeof SendMessage === 'function') {
                    if (arg === undefined || arg === null) {
                        SendMessage(gameObjectName, method);
                    } else {
                        SendMessage(gameObjectName, method, arg);
                    }
                }
            } catch (e) {
                console.error('YandexGames SendMessage', method, e);
            }
        };

        window.__ygSend = send;

        var start = function () {
            if (!window.ysdk) {
                setTimeout(start, 50);
                return;
            }

            var ysdk = window.ysdk;
            var player = null;
            var payments = null;

            ysdk.on('game_api_pause', function () {
                send('OnPlatformPause');
            });
            ysdk.on('game_api_resume', function () {
                send('OnPlatformResume');
            });

            var env = {
                lang: (ysdk.environment && ysdk.environment.i18n && ysdk.environment.i18n.lang) || 'ru',
                playerId: '',
                authorized: false
            };

            var loadSaves = function () {
                if (!player || !player.getData) {
                    send('OnJsCloudSaves', '');
                    return;
                }
                player.getData(['saves']).then(function (data) {
                    var payload = '';
                    if (data && data.saves) {
                        if (Array.isArray(data.saves) && data.saves.length > 0) {
                            payload = typeof data.saves[0] === 'string'
                                ? data.saves[0]
                                : JSON.stringify(data.saves[0]);
                        } else if (typeof data.saves === 'string') {
                            payload = data.saves;
                        }
                    }
                    send('OnJsCloudSaves', payload || '');
                }).catch(function (err) {
                    console.error('YandexGames getData', err);
                    send('OnJsCloudSaves', '');
                });
            };

            var loadPayments = function () {
                return ysdk.getPayments().then(function (p) {
                    payments = p;
                    window.__ygPayments = payments;
                    return Promise.all([payments.getCatalog(), payments.getPurchases()]);
                }).then(function (results) {
                    var products = results[0] || [];
                    var purchases = results[1] || [];
                    var items = [];
                    for (var i = 0; i < products.length; i++) {
                        var consumed = true;
                        for (var j = 0; j < purchases.length; j++) {
                            if (purchases[j].productID === products[i].id) {
                                consumed = false;
                                break;
                            }
                        }
                        items.push({
                            id: products[i].id,
                            title: products[i].title,
                            description: products[i].description,
                            price: products[i].price,
                            priceValue: products[i].priceValue,
                            priceCurrencyCode: products[i].priceCurrencyCode,
                            consumed: consumed
                        });
                    }
                    send('OnJsCatalog', JSON.stringify({ items: items }));
                    for (var u = 0; u < purchases.length; u++) {
                        send('OnJsPurchaseSuccess', purchases[u].productID);
                    }
                }).catch(function (err) {
                    console.log('YandexGames payments unavailable', err);
                    send('OnJsCatalog', '{"items":[]}');
                });
            };

            ysdk.getPlayer().then(function (p) {
                player = p;
                window.__ygPlayer = player;
                env.playerId = player.getUniqueID ? player.getUniqueID() : '';
                env.authorized = player.isAuthorized ? !!player.isAuthorized() : false;
                send('OnJsEnvironment', JSON.stringify(env));
                return loadPayments();
            }).catch(function (err) {
                console.error('YandexGames getPlayer', err);
                send('OnJsEnvironment', JSON.stringify(env));
                send('OnJsCatalog', '{"items":[]}');
            }).then(function () {
                loadSaves();
            });
        };

        start();
    },

    YandexGames_SaveCloud: function (jsonPtr, flush) {
        var json = UTF8ToString(jsonPtr);
        var player = window.__ygPlayer;
        if (!player || !player.setData) {
            console.error('YandexGames SaveCloud: no player');
            return;
        }
        player.setData({ saves: [json] }, flush === 1);
    },

    YandexGames_SetLeaderboard: function (namePtr, score) {
        var name = UTF8ToString(namePtr);
        if (!window.ysdk || !window.ysdk.leaderboards) return;
        window.ysdk.leaderboards.setScore(name, score);
    },

    YandexGames_GetLeaderboard: function (namePtr, quantityTop, quantityAround, includeUser) {
        var name = UTF8ToString(namePtr);
        var send = window.__ygSend;
        if (!window.ysdk || !window.ysdk.leaderboards || !send) return;

        window.ysdk.leaderboards.getEntries(name, {
            quantityTop: quantityTop,
            quantityAround: quantityAround,
            includeUser: includeUser === 1
        }).then(function (res) {
            var players = [];
            var entries = (res && res.entries) || [];
            for (var i = 0; i < entries.length; i++) {
                var entry = entries[i];
                var player = entry.player || {};
                var allowed = player.scopePermissions && player.scopePermissions.public_name === 'allow';
                players.push({
                    rank: entry.rank,
                    name: allowed ? (player.publicName || '') : '---',
                    score: entry.score,
                    photo: player.getAvatarSrc ? player.getAvatarSrc('medium') : '',
                    uniqueID: player.uniqueID || ''
                });
            }
            send('OnJsLeaderboard', JSON.stringify({
                technoName: name,
                currentPlayerRank: (res && res.userRank) || 0,
                players: players
            }));
        }).catch(function (err) {
            console.error('YandexGames getEntries', err);
        });
    },

    YandexGames_ShowInterstitial: function () {
        var send = window.__ygSend;
        if (!window.ysdk || !window.ysdk.adv) {
            if (send) send('OnJsInterstitialClosed', 'false');
            return;
        }
        window.ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function (wasShown) {
                    if (send) send('OnJsInterstitialClosed', wasShown ? 'true' : 'false');
                },
                onError: function () {
                    if (send) send('OnJsInterstitialClosed', 'false');
                }
            }
        });
    },

    YandexGames_ShowRewarded: function (rewardIdPtr) {
        var rewardId = UTF8ToString(rewardIdPtr);
        var send = window.__ygSend;
        if (!window.ysdk || !window.ysdk.adv) {
            if (send) send('OnJsRewardedClosed');
            return;
        }
        window.ysdk.adv.showRewardedVideo({
            callbacks: {
                onRewarded: function () {
                    if (send) send('OnJsRewarded', rewardId);
                },
                onClose: function () {
                    if (send) send('OnJsRewardedClosed');
                },
                onError: function () {
                    if (send) send('OnJsRewardedClosed');
                }
            }
        });
    },

    YandexGames_Purchase: function (productIdPtr) {
        var productId = UTF8ToString(productIdPtr);
        var send = window.__ygSend;
        var payments = window.__ygPayments;
        if (!payments) {
            if (send) send('OnJsPurchaseFailed', productId);
            return;
        }
        payments.purchase({ id: productId }).then(function () {
            payments.getPurchases().then(function (purchases) {
                for (var i = 0; i < purchases.length; i++) {
                    if (purchases[i].productID === productId) {
                        payments.consumePurchase(purchases[i].purchaseToken);
                    }
                }
                if (send) send('OnJsPurchaseSuccess', productId);
            }).catch(function () {
                if (send) send('OnJsPurchaseSuccess', productId);
            });
        }).catch(function (err) {
            console.error('YandexGames purchase', err);
            if (send) send('OnJsPurchaseFailed', productId);
        });
    },

    YandexGames_ConsumePurchase: function (productIdPtr) {
        var productId = UTF8ToString(productIdPtr);
        var send = window.__ygSend;
        var payments = window.__ygPayments;
        if (!payments) return;
        payments.getPurchases().then(function (purchases) {
            for (var i = 0; i < purchases.length; i++) {
                if (purchases[i].productID === productId) {
                    payments.consumePurchase(purchases[i].purchaseToken);
                    if (send) send('OnJsPurchaseSuccess', productId);
                }
            }
        }).catch(function (err) {
            console.error('YandexGames consume', err);
        });
    },

    YandexGames_LoadingReady: function () {
        if (window.ysdk && window.ysdk.features && window.ysdk.features.LoadingAPI) {
            window.ysdk.features.LoadingAPI.ready();
        }
    },

    YandexGames_GameplayStart: function () {
        if (window.ysdk && window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.start();
        }
    },

    YandexGames_GameplayStop: function () {
        if (window.ysdk && window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.stop();
        }
    }
});
