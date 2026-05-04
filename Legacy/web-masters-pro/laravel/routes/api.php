<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Models\Permission;
use App\Http\Controllers\AuthController;
use App\Http\Controllers\UserController;
use App\Http\Controllers\ProfessionControler;
use App\Http\Controllers\RegisterController;
use App\Http\Controllers\WorkController;
use App\Http\Controllers\OrderController;
use App\Http\Controllers\PaymentController;
use App\Http\Controllers\SupportController;
use App\Http\Controllers\StatisticController;
use App\Http\Controllers\PlatformSettingController;

Route::get('/permission/list', function (Request $request) {
    return Permission::select('model', 'action')
            ->distinct()
            ->get()->groupBy('model')
            ->map(function ($actions, $model) {
                return $actions->pluck('action')->toArray(); 
            });
});

Route::middleware(['auth:sanctum'])->post('/broadcasting/auth', function (Request $request) {
    $socketId = $request->input('socket_id');
    $channelName = $request->input('channel_name');

    $stringToAuth = $socketId . ':' . $channelName;
    $hashed = hash_hmac('sha256', $stringToAuth, env('REVERB_APP_SECRET'));

    return ['auth' => env('REVERB_APP_KEY') . ':' . $hashed];
});

/*** AuthController ***/

Route::post('/auth/login', [AuthController::class, 'login']);
Route::post('/auth/sms/login', [AuthController::class, 'loginBySmsCode']);
Route::post('/auth/logout', [AuthController::class, 'logout']);

Route::post('/auth/sms/send', [AuthController::class, 'sendSmsCode']);

/**********************/

/*** RegisterController ***/

Route::post('/register/confirm/phone', [RegisterController::class, 'confirmPhone']);
Route::post('/register/worker/confirm/form', [RegisterController::class, 'confirmWorkerForm']);
Route::post('/register/worker/finish', [RegisterController::class, 'registerWorker']);
Route::post('/register/customer', [RegisterController::class, 'registerCustomer']);

Route::post('/register/sms/send', [RegisterController::class, 'sendSmsCode']);

/**************************/

/*** UserController ***/

Route::middleware(['auth:sanctum'])->get('/user', [UserController::class, 'getUser']);
Route::middleware(['auth:sanctum'])->get('/user/notifications', [UserController::class, 'getUserNotifications']);
Route::middleware(['auth:sanctum'])->get('/user/tickets', [UserController::class, 'getUserTickets']);
Route::middleware(['auth:sanctum', 'permission:Worker.read'])->get('/user/list/worker', [UserController::class, 'loadWorkersList']);
Route::middleware(['auth:sanctum', 'permission:Customer.read'])->get('/user/list/customer', [UserController::class, 'loadCustomersList']);
Route::middleware(['auth:sanctum', 'permission:File.read'])->get('/user/{userId}/file/{fileId}', [UserController::class, 'getUserFile']);

Route::middleware(['auth:sanctum', 'permission:User.save'])->post('/user/save', [UserController::class, 'saveUser']);
Route::middleware(['auth:sanctum', 'permission:User.save'])->post('/user/{userId}/customer/edit', [UserController::class, 'editCustomer']);
Route::middleware(['auth:sanctum', 'permission:User.save'])->post('/user/list', [UserController::class, 'loadFilteredUserList']);

Route::middleware(['auth:sanctum', 'permission:Worker.save'])->post('/user/worker/{workerId}/edit/profession', [UserController::class, 'editWorkerProfessions']);
Route::middleware(['auth:sanctum', 'permission:Worker.save'])->post('/user/worker/{workerId}/edit/payment-requisites', [UserController::class, 'editWorkerPaymentRequisites']);
Route::middleware(['auth:sanctum'])->post('/user/edit/payment-requisites', [UserController::class, 'editUserPaymentRequisites']);
Route::middleware(['auth:sanctum', 'permission:Worker.save'])->post('/user/worker/{workerId}/edit/personal-data', [UserController::class, 'editWorkerPersonalData']);

Route::middleware(['auth:sanctum', 'permission:User.delete'])->delete('/user/{userId}/delete', [UserController::class, 'deleteUser']);
Route::middleware(['auth:sanctum'])->delete('/user/notifications', [UserController::class, 'deleteUserNotifications']);

/**********************/

/*** ProfessionControler ***/

Route::middleware(['auth:sanctum', 'permission:Profession.read'])->get('/profession/list/detailed', [ProfessionControler::class, 'getProfessionListDetailed']);
Route::get('/profession/list/select', [ProfessionControler::class, 'getProfessionListSelect']);

Route::middleware(['auth:sanctum', 'permission:Profession.save'])->post('/profession/save', [ProfessionControler::class, 'saveProfession']);
Route::middleware(['auth:sanctum', 'permission:Profession.save'])->post('/profession/level/save', [ProfessionControler::class, 'saveProfessionLevel']);

Route::middleware(['auth:sanctum', 'permission:Profession.delete'])->post('/profession/level/delete', [ProfessionControler::class, 'deleteProfessionLevel']);
Route::middleware(['auth:sanctum', 'permission:Profession.delete'])->post('/profession/delete', [ProfessionControler::class, 'deleteProfession']);

/***************************/

/***WorkController ***/

Route::middleware(['auth:sanctum', 'permission:Work.accept'])->get('/work/check/accept', [WorkController::class, 'checkAcceptWork']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->get('/work/accept', [WorkController::class, 'getAcceptWork']);

Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/accept', [WorkController::class, 'acceptWorkOrder']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/refuse', [WorkController::class, 'refuseWorkOrder']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/not-met', [WorkController::class, 'workerNotMeet']);
//Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/address/accept', [WorkController::class, 'acceptWorkAddress']);
//Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/meeting/accept', [WorkController::class, 'acceptWorkMeeting']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/shift/{workShiftId}/meeting/accept', [WorkController::class, 'acceptWorkShiftMeeting']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/shift/{workShiftId}/address/accept', [WorkController::class, 'acceptWorkShiftAddress']);
Route::middleware(['auth:sanctum', 'permission:Work.accept'])->post('/work/{workId}/shift/{workShiftId}/report', [WorkController::class, 'createReport']);
Route::middleware(['auth:sanctum', 'permission:Report.accept'])->post('/work/report/{reportId}/confirm/accept', [WorkController::class, 'acceptReport']);
Route::middleware(['auth:sanctum', 'permission:Report.accept'])->post('/work/report/{reportId}/confirm/refuse', [WorkController::class, 'refuseReport']);

/********************/

/*** OrderController ***/

Route::middleware(['auth:sanctum', 'permission:Order.place'])->get('/order/user/{userId}/list', [OrderController::class, 'getUserOrdersList']);
Route::middleware(['auth:sanctum', 'permission:Order.read'])->get('/order/list', [OrderController::class, 'getOrdersList']);
Route::middleware(['auth:sanctum'])->get('/order/{orderId}/get', [OrderController::class, 'getOrder']);
Route::middleware(['auth:sanctum'])->get('/order/{orderId}/worker/{workerId}/file/{fileId}/get', [OrderController::class, 'getWorkerFile']);
Route::middleware(['auth:sanctum'])->get('/order/{orderId}/worker/{workerId}/contract/get', [OrderController::class, 'getWorkerContract']);
Route::middleware(['auth:sanctum'])->get('/order/{orderId}/download/excel', [OrderController::class, 'downloadOrderDetailExcel']);

Route::middleware(['auth:sanctum', 'permission:Order.place'])->post('/order/place', [OrderController::class, 'placeNewOrder']);
Route::middleware(['auth:sanctum', 'permission:Order.place'])->post('/order/{orderId}/worker/accept', [OrderController::class, 'acceptWorker']);
Route::middleware(['auth:sanctum', 'permission:Order.place'])->post('/order/{orderId}/worker/refuse', [OrderController::class, 'refuseWorker']);

/***********************/

/*** SupportController ***/

Route::middleware(['auth:sanctum', 'permission:Ticket.read'])->get('/support/ticket/list', [SupportController::class, 'getSupportTicketsList']);
Route::middleware(['auth:sanctum', 'permission:Ticket.read'])->get('/support/ticket/{ticketId}/get', [SupportController::class, 'getSupportTicketsById']);

Route::middleware(['auth:sanctum', 'permission:Ticket.create'])->post('/support/ticket/create', [SupportController::class, 'createTicket']);
Route::middleware(['auth:sanctum', 'permission:Ticket.send'])->post('/support/ticket/{ticketId}/message/send', [SupportController::class, 'sendTicketMessage']);
Route::middleware(['auth:sanctum', 'permission:Ticket.send'])->post('/support/ticket/{ticketId}/status/closed', [SupportController::class, 'closeTicket']);

/*************************/

/*** StatisticController ***/

Route::middleware(['auth:sanctum', 'permission:App.stats'])->get('/stats/get', [StatisticController::class, 'getStatistics']);

/***************************/

/*** PaymentController ***/

Route::get('/payment/test', [PaymentController::class, 'test']);
Route::middleware(['auth:sanctum', 'permission:PaymentRequest.read'])->get('/payment/list', [PaymentController::class, 'getPaymentRequestsList']);

/***************************/

/*** PlatformSettingController ***/

Route::middleware(['auth:sanctum', 'permission:PlatformSetting.read'])->get('/setting/list', [PlatformSettingController::class, 'getSettingsList']);

Route::middleware(['auth:sanctum', 'permission:PlatformSetting.save'])->post('/setting/save', [PlatformSettingController::class, 'saveSetting']);

/*********************************/
