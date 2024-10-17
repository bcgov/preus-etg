var path = require('path');
var webpack = require('webpack');

module.exports = {
  mode: "development",
  entry: {
    'external': './UI/javascript/external/index.js',
    'internal': './UI/javascript/internal/index.js',
    'participant-confirmation': './UI/javascript/part/index.js',
  },
  output: {
    path: path.join(__dirname, '../js'),
    publicPath: 'js/',
    filename: '[name].js'
  },
  module: {
    rules: [
      {
        test: /\.css$/,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader" },
        ]
      },
      {
        test: /\.(jpe?g|png|gif|svg)$/i,
        use: [
          {
            loader: "url-loader",
            options: {
              limit: 100000
            }
          }
        ]
      }
    ]
  },
  optimization: {
    splitChunks: {
      cacheGroups: {
        commons: {
          test: /[\\/]node_modules[\\/]/,
          name: 'common',
          chunks: 'all'
        }
      }
    }
  },
  plugins: [
    new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
  ],
  externals: {
    jquery: 'jQuery',
  },
  devtool: "source-map",
};
