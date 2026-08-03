import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

const iconSx = {
	width: 32,
	height: 32,
	"& rect": {
		fill: "none",
		stroke: "var(--mui-palette-contrastingSecondary-main)",
		strokeWidth: 1.5,
	},
} as const

const CheckboxCircleUncheckedIcon = ({ sx, ...props }: SvgIconProps) => (
	<SvgIcon
		{...props}
		viewBox="0 0 16 16"
		sx={[iconSx, ...(Array.isArray(sx) ? sx : [sx])]}
	>
		<rect x="2" y="2" width="12" height="12" rx="6" />
	</SvgIcon>
)

export default CheckboxCircleUncheckedIcon
