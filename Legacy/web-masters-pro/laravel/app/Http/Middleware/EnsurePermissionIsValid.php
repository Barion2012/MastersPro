<?php

namespace App\Http\Middleware;

use App\Models\Permission;
use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Symfony\Component\HttpFoundation\Response;

class EnsurePermissionIsValid
{
    /**
     * Handle an incoming request.
     *
     * @param  \Closure(\Illuminate\Http\Request): (\Symfony\Component\HttpFoundation\Response)  $next
     */
    public function handle(Request $request, Closure $next, $code): Response
    {
        $code = explode('.', $code);
        $model = $code[0];
        $action = $code[1];

        $permission = Permission::where([['role', Auth::user()->role], ['model', $model], ['action', $action]])->first();

        if (!empty($permission))
        {
            return $next($request);
        }

        return abort(403, 'Доступ запрещен');
    }
}
