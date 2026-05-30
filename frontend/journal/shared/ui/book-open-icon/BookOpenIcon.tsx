import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

const iconSx = {
	width: 64,
	height: 64,
	"& path": {
		fill: "var(--mui-palette-secondary-light)",
		stroke: "var(--mui-palette-contrastingSecondary-main)",
		strokeWidth: 3,
		strokeLinecap: "round",
		strokeLinejoin: "round",
	},
} as const

const BookOpenIcon = ({ sx, ...props }: SvgIconProps) => (
	<SvgIcon
		{...props}
		viewBox="0 0 64 64"
		sx={[iconSx, ...(Array.isArray(sx) ? sx : [sx])]}
	>
		<path d="M31.9999 18.6667C31.9999 15.8377 30.8761 13.1246 28.8757 11.1242C26.8753 9.12381 24.1622 8 21.3333 8H5.33325V48H23.9999C26.1216 48 28.1565 48.8429 29.6568 50.3431C31.1571 51.8434 31.9999 53.8783 31.9999 56M31.9999 18.6667V56M31.9999 18.6667C31.9999 15.8377 33.1237 13.1246 35.1241 11.1242C37.1245 9.12381 39.8376 8 42.6666 8H58.6666V48H39.9999C37.8782 48 35.8434 48.8429 34.3431 50.3431C32.8428 51.8434 31.9999 53.8783 31.9999 56" />
	</SvgIcon>
)

export default BookOpenIcon
