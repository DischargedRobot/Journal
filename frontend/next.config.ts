import type { NextConfig } from "next"
const path = require('path')
const nextConfig: NextConfig = {
	/* config options here */
	reactCompiler: true,
	experimental: {
		optimizePackageImports: ["@chakra-ui/react"],
	},

	turbopack: {
		root: path.join(__dirname, '..'),
	  },
}

export default nextConfig
