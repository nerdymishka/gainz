
const path = require("path");
const fs  = require("fs");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const appRoot = fs.realpathSync(process.cwd())

const resolvePath = relativePath => path.resolve(appRoot, relativePath);
const host = process.env.HOST || "localhost";
 

module.exports = {

    mode: 'development',
    entry: resolvePath("src"),

    output: {
        filename: "public/js/bundle.js"
    },

    plugins: [
        new HtmlWebpackPlugin({
            inject: true,
            template: resolvePath("public/controls.html"),
            filename: "controls.html" 
        }),
        new HtmlWebpackPlugin({
            inject: true,
            template: resolvePath("public/index.html")
        })
    ],

    module: {
        rules: [
            {
                test: /\.(scss)$/,
                use: [
                    {loader: 'style-loader'},
                    {loader: 'css-loader'},
                    {
                        loader: 'postcss-loader',
                        options: {
                            "plugins": function() {return require("autoprefixer") }
                        }
                    },
                    {loader: 'sass-loader' }
                ]
            }
        ]
    },

    devServer: {
        "contentBase": resolvePath('public'),
        'compress': true,
        "hot":true,
        "host": host,
        "port": 3000,
        "publicPath": "/"
    }
}