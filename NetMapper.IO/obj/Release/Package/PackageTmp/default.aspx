<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="NetMapper.IO._default" %>

<!doctype html>
<html lang="en" class="fullscreen-bg">
<head>
    <title>NetMapper.IO</title>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0">
    <link rel="stylesheet" href="/assets/theme/css/bootstrap.min.css">
    <link rel="stylesheet" href="/assets/theme/vendor/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="/assets/theme/vendor/linearicons/style.css">
    <link rel="stylesheet" href="/assets/theme/css/main.css">
    <link href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700" rel="stylesheet">
    <link rel="apple-touch-icon" sizes="76x76" href="/assets/theme/img/apple-icon.png">
    <link href="/assets/css/toastr/toastr.min.css" rel="stylesheet" />

</head>

<body>
    <div id="wrapper">
        <div class="vertical-align-wrap">
            <div class="vertical-align-middle">
                <div class="auth-box ">
                    <div class="left hidden" id="usr-signup">
                        <div class="content">
                            <div class="header">
                                <h3><strong>NetMapper.IO</strong></h3>
                                <p class="lead">Create an account</p>
                            </div>
                            <form class="form-auth-small" id="frm-create">
                                <input type="hidden" class="hidden" name="reqType" value="userSignup" />
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="forename" class="control-label sr-only">Username</label>
                                        <input type="text" class="form-control" name="forename" placeholder="Forename" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="surname" class="control-label sr-only">Surname</label>
                                        <input type="text" class="form-control" name="surname" placeholder="Surname" />
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label for="username" class="control-label sr-only">Usename</label>
                                        <input type="text" class="form-control" name="username" placeholder="Username" />
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label for="email" class="control-label sr-only">Email Address</label>
                                        <input type="text" class="form-control" name="email" placeholder="Email Address" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="password1" class="control-label sr-only">Password</label>
                                        <input type="password" class="form-control" name="password1" placeholder="Password" />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="password2" class="control-label sr-only">Repeat Password</label>
                                        <input type="password" class="form-control" name="password2" placeholder="Repeat Password" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <button type="button" class="btn btn-block btn-default login-account"><i class="fa fa-sign-in"></i>&nbsp;Go to Login</button>
                                    </div>
                                    <div class="col-md-6">
                                        <button type="button" class="btn btn-block btn-primary" id="create-btn"><i class="fa fa-user-plus"></i>&nbsp;Create Account</button>
                                    </div>
                                </div>
                                <div class="bottom">
                                    <span class="helper-text"><a href="#" class="forgot-password"><i class="fa fa-lock"></i>&nbsp;Forgot password?</a></span>
                                </div>
                            </form>
                        </div>
                    </div>
                    <div class="left" id="usr-login">
                        <div class="content">
                            <div class="header">
                                <h3><strong>NetMapper.IO</strong></h3>
                                <p class="lead">Login to your account</p>
                            </div>
                            <form id="frm-login" class="form-auth-small">
                                <input type="hidden" class="hidden" name="reqType" value="userLogin" />
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label for="signin-email" class="control-label sr-only">Username</label>
                                        <input type="text" id="username" class="form-control" name="username" placeholder="Username" />
                                    </div>
                                    <div class="form-group">
                                        <label for="signin-password" class="control-label sr-only">Password</label>
                                        <input type="password" id="password" class="form-control" name="password" placeholder="Password" />
                                    </div>
                                    <div class="form-group clearfix">
                                        <label class="fancy-checkbox element-left">
                                            <input type="checkbox">
                                            <span>Remember me</span>
                                        </label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <button type="button" class="btn btn-block btn-default create-account"><i class="fa fa-user-plus"></i>&nbsp;Create Account</button>
                                    </div>
                                    <div class="col-md-6">
                                        <button type="button" class="btn btn-block btn-primary" id="login-btn"><i class="fa fa-sign-in"></i>&nbsp;Login</button>
                                    </div>
                                </div>
                                <div class="bottom">
                                    <span class="helper-text"><a href="#" class="forgot-password"><i class="fa fa-lock"></i>&nbsp;Forgot password?</a></span>
                                </div>
                            </form>
                        </div>
                    </div>
                    <div class="right">
                        <div class="overlay"></div>
                        <div class="content text">
                            <h1 class="heading">NetMapper.IO</h1>
                            <p>A Simple Network Topology Design and Collaboration Tool</p>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <script src="/assets/js/jQuery/jquery-3.3.1.min.js"></script>
    <script src="/assets/js/toastr/toastr.min.js"></script>
    <script src="/assets/js/App/default.js"></script>
</body>
</html>
